using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using LgWG.LogQuery.LogTrace;
using LgWG.LogQuery.LogMonitor;
using LgWG.LogQuery.LogMonitor.DTO;
using System;
using LgWG.LogQuery.LogTrace.DTO;
using Abp.Application.Services.Dto;

namespace LgWG.LogQuery.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : LogQueryControllerBase
    {
        private readonly ILog_OperateTraceService _logTraceService;
        private readonly ILog_SystemMonitorService _logMonitorService;

        public HomeController(ILog_OperateTraceService logTraceService, ILog_SystemMonitorService logMonitorService)
        {
            _logTraceService = logTraceService;
            _logMonitorService = logMonitorService;
        }

        public ActionResult Index()
        {       
            return View();
        }


    }
}