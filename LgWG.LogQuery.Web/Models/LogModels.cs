using Abp.AutoMapper;
using LgWG.LogQuery.LogMonitor.DTO;
using LgWG.LogQuery.LogTrace.DTO;
using Log2Net.Models;
using Log2Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace LgWG.LogQuery.Web.Models
{
    [AutoMapFrom(typeof(Log_OperateTraceDto))]
    public class Log_OperateTraceView : Log_OperateTraceDto
    {
        public string LogTypeCN { get { return LogType.ToString(); } }   //Enum.GetName(typeof(LogType), LogType));

        public string SysName { get { return Configuration.SysNameMap.GetMySystemName(SystemID); } }
    }


    [AutoMapFrom(typeof(Log_SystemMonitorDto))]
    public class Log_OperateMonitorView : Log_SystemMonitorDto
    {

        public string SysName { get { return Configuration.SysNameMap.GetMySystemName(SystemID); } }
    }

    public class MonitorSearchVM
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string sortby { get; set; }
        public string sortway { get; set; }
        public SysCategory app { get; set; }
        public string host { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string other { get; set; }

    }

    public class TraceSearchVM
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string sortby { get; set; }
        public string sortway { get; set; }
        public SysCategory app { get; set; }
        public string host { get; set; }
        public LogType type { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string userName { get; set; }
        public string keyWord { get; set; }
        public string modTab { get; set; }
    }

    public class IndexView
    {
        public IList<ApplicationData> Apps
        {
            get;
            set;
        }
    }


    public class LogTraceVM
    {
        public LogType LogType { get { return _LogType; } set { _LogType = value; } } //日志类型
        public string TabOrModu { get { return _TabOrModu; } set { _TabOrModu = value; } }//表名或模块名称
        public string Detail { get; set; } //日志内容
        public string Remark { get; set; } //其他信息

        string _TabOrModu = "启动";
        LogType _LogType = LogType.业务记录;
    }

}