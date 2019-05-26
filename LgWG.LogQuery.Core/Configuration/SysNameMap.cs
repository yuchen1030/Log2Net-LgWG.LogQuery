using Log2Net.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.Configuration
{
    public class SysNameMap
    {
        public static string GetMySystemName(Log2Net.Models.SysCategory sysCategory)
        {
            try
            {
                var dic = Log2Net.LogApi.GetLogWebApplicationsName();
                return dic[sysCategory];
            }
            catch
            {
                var curEnum =(int)sysCategory;
                return sysCategory + "__" + curEnum;
            }
        }



    }



}
