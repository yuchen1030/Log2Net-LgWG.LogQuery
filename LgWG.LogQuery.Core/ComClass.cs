using LgWG.LogQuery.Configuration;
using Log2Net.Models;
using Log2Net.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;

namespace LgWG.LogQuery
{
    public class ComClass
    {

        //获取某个枚举的键值对
        public static Dictionary<string, int> GetDicFromEnumType(object enumType)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<string> result = new List<string>();
            try
            {
                var arrayEnum = Enum.GetValues(enumType.GetType());
                foreach (var myCode in arrayEnum)
                {
                    try
                    {
                        string strVaule = myCode.ToString();//获取名称   //   string strName = Enum.GetName(typeof(UserGroups), myCode);//获取名称
                        result.Add(strVaule);
                        dic.Add(strVaule, (int)myCode);
                    }
                    catch
                    {

                    }
                }
                dic = dic.OrderBy(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
            }
            catch
            {

            }
            return dic;
        }

        //获取本系统中使用的系统枚举
        public static List<SysCategory> GetMySysCategory()
        {
            var dic = Log2Net.LogApi.GetLogWebApplicationsName();
            return  dic.Keys.ToList();
        }


        public static string StaticBaseUrl = "";
        public static string GetWebBaseUrl()
        {
            try
            {
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                return baseUrl;
            }
            catch
            {
                return StaticBaseUrl;
            }
        }

        static List<T> GetEnumListFromEnumType<T>(object enumType)
        {
            List<T> list = new List<T>();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<string> result = new List<string>();
            try
            {
                var arrayEnum = Enum.GetValues(enumType.GetType());
                foreach (var myCode in arrayEnum)
                {
                    list.Add((T)myCode);
                    string strVaule = myCode.ToString();//获取名称   //   string strName = Enum.GetName(typeof(UserGroups), myCode);//获取名称
                    result.Add(strVaule);
                    dic.Add(strVaule, (int)myCode);
                }
                dic = dic.OrderBy(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
            }
            catch
            {

            }
            list = list.OrderBy(a => a).ToList();
            return list;
        }

    }

    public class WebApiHelper
    {
        enum JsonType
        {
            Json_Error,
            Json_Object,
            Json_Array,
            Json_String,
        }



        //判断字符串是是JsonObject还是JsonArray
        JsonType GetJSonType(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return JsonType.Json_Error;
            }
            string char1st = str.Substring(0, 1);
            if (char1st == "[")
            {
                return JsonType.Json_Array;
            }
            else if (char1st == "{")
            {
                return JsonType.Json_Object;
            }
            else if (char1st == "\"")
            {
                return JsonType.Json_String;
            }
            return JsonType.Json_Error;
        }

        // //{"result":["A12210289811","apiTest","OATEST"],"targetUrl":null,"success":true,"error":null,"unAuthorizedRequest":false,"__abp":true}

        class AbpObject
        {
            public object result { get; set; }
            public bool __abp { get; set; }
        }



        List<T> GetListFromJsonString<T>(string result, out string resultMsg)
        {
            resultMsg = "";
            if (string.IsNullOrEmpty(result))
            {
                return new List<T>();
            }
            if (result.Contains(",\"__abp\":"))
            {
                // result =  GetAbpStr();
                object result2 = SerializeHelper.DeserializeToObject<AbpObject>(result.ToString()).result;
                result = result2.ToString();
            }
            if (GetJSonType(result) == JsonType.Json_Array)
            {
                IEnumerable<T> resultT = SerializeHelper.DeserializeToObject<IEnumerable<T>>(result.ToString());
                return resultT.ToList();
            }
            else if (GetJSonType(result) == JsonType.Json_Object)
            {
                T resultT = SerializeHelper.DeserializeToObject<T>(result.ToString());
                return new List<T>() { resultT };
            }
            else if (GetJSonType(result) == JsonType.Json_String)
            {
                result = result.Trim('"');
                var t = new List<string>() { result.ToString() };
                return t as List<T>;
            }
            else
            {
                resultMsg = result + "字符串无法解析";
                return new List<T>();
            }
        }

        public List<T> GetDataList<T>(string baseUrl, string ticket, string requestMethod, out string resultMsg)
        {
            resultMsg = "";
            try
            {
                var result = HttpClientDoGet<T>(baseUrl, ticket, requestMethod, out resultMsg);
                return GetListFromJsonString<T>(result, out resultMsg);
            }
            catch (Exception ex)
            {
                Log2Net.LogApi.WriteExceptLog(ex, "WebApiHelper.GetDataList");
                resultMsg = ex.Message;
                return new List<T>();
            }
        }

        string HttpClientDoGet<T>(string baseUrl, string ticket, string requestMethod, out string resultMsg)
        {
            resultMsg = "";

            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = ComClass.GetWebBaseUrl();
            }
            if (string.IsNullOrEmpty(ticket))
            {
                // ticket = BasicAuthenticationAttribute.webApiAccessKey;//本网站使用时，不必放在配置文件中
            }

            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ticket);
                    HttpResponseMessage response = client.GetAsync(requestMethod).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var resultStr = response.Content.ReadAsStringAsync();//返回的是json字符串，可反序列化得到对象
                        //  IEnumerable<T> resultModel = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                        resultMsg = "OK";
                        return resultStr.Result;
                    }
                    else
                    {
                        int code = (int)response.StatusCode;
                        string msg = response.ReasonPhrase;
                        resultMsg = msg;
                        return SerializeHelper.SerializeToString(new List<T>());
                    }
                }
            }
            catch (Exception ex)
            {
                Log2Net.LogApi.WriteExceptLog(ex, "WebApiHelper.HttpClientDoGet");
                resultMsg = ex.Message;
                return SerializeHelper.SerializeToString(new List<T>());
            }

        }






        public List<V> HttpClientDoPost<T, V>(string baseUrl, string ticket, string requestMethod, T model, out string resultMsg) //where V : new()
        {
            resultMsg = "";
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = ComClass.GetWebBaseUrl();
            }
            if (string.IsNullOrEmpty(ticket))
            {
                // ticket = BasicAuthenticationAttribute.webApiAccessKey; //本网站使用，不必放在配置文件中
            }

            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            // var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            try
            {
                using (var httpclient = new HttpClient(handler))
                {
                    httpclient.BaseAddress = new Uri(baseUrl);
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ticket);
                    //   httpclient.DefaultRequestHeaders.Add("Content-Type", "application/json;charset=UTF-8");                 
                    Dictionary<string, object> dic = GetPropertity(model);
                    var content = new FormUrlEncodedContent(dic.ToDictionary(k => k.Key, v => v.Value != null ? v.Value.ToString() : ""));
                    var response = httpclient.PostAsync(requestMethod, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = response.Content.ReadAsStringAsync();

                        var resultT = GetListFromJsonString<V>(responseString.Result.ToString(), out resultMsg);
                        //  V resultT = Common.SerializerHelper.DeserializeToObject<V>(responseString);
                        return resultT;
                    }
                    else
                    {
                        int code = (int)response.StatusCode;
                        resultMsg = "执行失败code=" + code;
                        // return default(V);
                        return new List<V>();
                        //  return resultT response.ReasonPhrase;

                    }
                }
            }
            catch (Exception ex)
            {
                Log2Net.LogApi.WriteExceptLog(ex, "WebApiHelper.HttpClientDoPost");
                resultMsg = ex.Message;
                // return default(V);
                return new List<V>();
            }
        }


        public Dictionary<string, object> GetPropertity(object model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (model == null)
            {
                return dic;
            }
            List<string> list = new List<string>();

            Type t = model.GetType();
            //Type t = typeof(   Login );
            // FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            //string msg = "property : ";
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object obj = property.GetValue(model, null);
                dic.Add(property.Name, property.GetValue(model, null));
                //list.Add(property.Name);
            }
            return dic;
        }






    }

    public class SerializeHelper
    {
        public static string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }


        public static T DeserializeToObject<T>(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                return default(T);
            }
        }


        public static T XMLDESerializer<T>(string strXML) where T : class
        {
            //  return DESerializer2<T>(strXML);
            try
            {
                var typeClass = typeof(T);
                var xmlRootName = "ArrayOf" + typeClass.GenericTypeArguments[0].Name;
                strXML = strXML.Replace("<DocumentElement>", "<" + xmlRootName + ">").Replace("</DocumentElement>", "</" + xmlRootName + ">")
                    .Replace("<DocumentElement />", "<" + xmlRootName + " />");

                using (StringReader sr = new StringReader(strXML))
                {
                    //设置Position属性代码：
                    //sr.Position = 0;
                    //sr.Seek(0, SeekOrigin.Begin);

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch //(Exception ex)
            {
                return null;
            }
        }

    }

}
