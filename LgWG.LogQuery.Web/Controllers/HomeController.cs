using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using LgWG.LogQuery.LogTrace;
using LgWG.LogQuery.LogMonitor;
using LgWG.LogQuery.LogMonitor.DTO;
using System;
using LgWG.LogQuery.LogTrace.DTO;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.Web.Models;
using Log2Net.Models;
using LgWG.LogQuery.Users;
using System.Threading.Tasks;

namespace LgWG.LogQuery.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : LogQueryControllerBase
    {
        private readonly ILog_OperateTraceService _logTraceService;
        private readonly ILog_SystemMonitorService _logMonitorService;
        private readonly IUserAppService _userAppService;
        public HomeController(ILog_OperateTraceService logTraceService, ILog_SystemMonitorService logMonitorService, IUserAppService userAppService) : base(userAppService)
        {
            _logTraceService = logTraceService;
            _logMonitorService = logMonitorService;
            _userAppService = userAppService;
        }

        public ActionResult Index()
        {
            LogTraceVM log = new LogTraceVM() { LogType = LogType.业务记录, TabOrModu = "系统监控", Detail = "进入了页面" };
            //string msg = WriteLog(LogLevel.Info, log);
            var nums = Log2Net.LogApi.GetNumOfOnLineAllVisit();
            return View();
        }


        public ActionResult Test()
        {
            return View();
        }

    }
}