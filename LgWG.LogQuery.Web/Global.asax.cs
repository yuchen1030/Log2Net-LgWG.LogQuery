using System;
using System.Threading;
using Abp.Castle.Logging.Log4Net;
using Abp.Web;
using Abp.WebApi.Validation;
using Castle.Facilities.Logging;
using Log2Net;
using System.Collections.Generic;

namespace LgWG.LogQuery.Web
{
    public class MvcApplication : AbpWebApplication<LogQueryWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseAbpLog4Net().WithConfig(Server.MapPath("log4net.config"))
            );

            base.Application_Start(sender, e);

            Log2Net.LogApi.RegisterLogInitMsg(Log2Net.Models.SysCategory.SysA_01, Application);//日志系统注册


        }

        protected override void Application_Error(object sender, EventArgs e)
        {
            //访问一个页面时，AJAX 请求 ASP.NET Web API 。多次点击刷新，出错，提示"已取消一个任务"。
            //原因：页面加载 =》 ajax 触发 web api =》  页面刷新 =》web api 的任务被取消; web api 是异步编程, 任务取消时，会触发导常。
            //服务器初次启动无法访问数据库的错误不记录
            try
            {
                Exception ex = System.Web.HttpContext.Current.Server.GetLastError().GetBaseException();
                if (ex.GetType().Name .Contains( "TaskCanceledException" ) || ex.GetType().Name.Contains("AggregateException") || ex.Message.Contains("无法打开登录所请求的数据库"))
                {
                    System.Web.HttpContext.Current.Server.ClearError();
                }
                else
                {
                    Log2Net.LogApi.HandAndWriteException();
                }         

            }
            catch
            {

            }
            base.Application_Error(sender, e);
        }

        protected override void Application_End(object sender, EventArgs e)
        {
            try
            {
                Log2Net.LogApi.WriteServerStopLog();//写停止日志
            }
            catch
            {

            }
            base.Application_End(sender, e);
        }


        //健康检查引起的Session_Start，在线人数不能增加
        bool IsHealthCheck()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request["bHC"] == "1")
                {
                    return true;
                }
                else
                {
                    var refUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                    if (refUrl.Contains("bHC=1"))
                    {
                        return true;
                    }
                }
            }
            catch
            {

            }
            return false;
        }

        static List<string> hcSessions = new List<string>();  //健康检查导致的seesion。这种session在人数统计时要忽略。
        protected override void Session_Start(Object sender, EventArgs e)//客户端一连接到服务器上，这个事件就会发生
        {
            base.Session_Start(sender, e);
            if (hcSessions.Contains(Session.SessionID))
            {
                return;
            }
            if (IsHealthCheck())
            {
                WriteDebugTraceToFile("Session_Start HC");
                hcSessions.Add(Session.SessionID);
                return;
            }
            Application.Lock();//锁定后，只有这个Session能够会话        
            try
            {
                Log2Net.LogApi.IncreaseOnlineVisitNum();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Application.UnLock();//会话完毕后解锁
            }

            WriteDebugTraceToFile("Session_Start");
        }

        protected override void Session_End(object sender, EventArgs e)
        {
            base.Session_End(sender, e);
            if (hcSessions.Contains(Session.SessionID))
            {
                WriteDebugTraceToFile("Session_End HC");
                hcSessions.RemoveAll(a => a == Session.SessionID);
                return;
            }
            Application.Lock();
            Log2Net.LogApi.ReduceOnlineNum();
            Application.UnLock();
            WriteDebugTraceToFile("Session_End");
        }

        protected override void Application_BeginRequest(object sender, EventArgs e)
        {
            Log2Net.LogApi.WriteFirstVisitLog();//写初次访问日志
            ComClass.StaticBaseUrl = ComClass.GetWebBaseUrl();
            base.Application_BeginRequest(sender, e);
            WriteDebugTraceToFile("BeginRequest");
        }

        void WriteDebugTraceToFile(string flag = "")
        {
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.ToString();
                string refUrl = System.Web.HttpContext.Current.Request.UrlReferrer == null ? "UrlReferrer=null" : System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                int num = 0;
                try
                {
                    num = Convert.ToInt32(Application["OnLineUserCnt"].ToString());
                }
                catch
                {
                    num = -1;
                }
                flag = string.IsNullOrEmpty(flag) ? "" : flag + " ; ";
                string msg = DateTime.Now.ToString("HH:mm:ss.fff") + " : " + flag + url + " ; " + num + "人在线 " + refUrl;
                Log2Net.LogApi.WriteMsgToDebugFile<string>(msg);
            }
            catch
            {

            }
        }


    }
}
