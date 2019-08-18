using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using LgWG.LogQuery.Authorization;
using LgWG.LogQuery.LogMonitor;
using LgWG.LogQuery.LogMonitor.DTO;
using LgWG.LogQuery.LogTrace;
using LgWG.LogQuery.Roles;
using LgWG.LogQuery.Users;
using LgWG.LogQuery.Web.Models;
using Log2Net;
using Log2Net.Models;
using Log2Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LgWG.LogQuery.Web.Controllers
{
    [AbpMvcAuthorize]
    [AbpMvcAuthorize(PermissionNames.Pages_LogMonitor)]
    public class LogMonitorController : LogQueryControllerBase
    {
        // GET: LogMonitor

        private readonly ILog_OperateTraceService _logTraceService;
        private readonly ILog_SystemMonitorService _logMonitorService;
        private readonly IRoleAppService _roleAppService;
        private readonly IUserAppService _userAppService;

        public LogMonitorController(ILog_OperateTraceService logTraceService, ILog_SystemMonitorService logMonitorService, IRoleAppService roleAppService, IUserAppService userAppService) : base(userAppService)
        {
            _logTraceService = logTraceService;
            _logMonitorService = logMonitorService;
            _roleAppService = roleAppService;
            _userAppService = userAppService;
        }

        public async Task<ActionResult> Index()
        {
            LogTraceVM log = new LogTraceVM() { LogType = LogType.业务记录, TabOrModu = "系统监控", Detail = "进入了页面" };
            string msg = WriteLog(LogLevel.Info, log);
            return View();
        }

        //获取各个服务器指定范围内的数据，用于曲线显示
        public ActionResult GetMonitorChartData(string range, long userId)
        {
            LogMonitorVM vm = _logMonitorService.GetMonitorChartData(range, userId);
            return AbpJson(vm, null, null, JsonRequestBehavior.AllowGet, true, false);
        }

        //获取所有的服务器的下拉列表
        public ActionResult GetAllServerNameList()
        {
            var sers = _logMonitorService.GetAllServerList();
            List<SelectListItem> tar = new List<SelectListItem>();
            foreach (var item in sers)
            {
                tar.Add(new SelectListItem() { Text = item, Value = item });
            }
            return AbpJson(tar, null, null, JsonRequestBehavior.AllowGet, true, false);
        }

        //获取所有的系统名称的下拉列表
        public ActionResult GetAllApplicationList()
        {
            List<SelectListItem> tar = new List<SelectListItem>();
            var enumList = _roleAppService.GetSysCategoryAccordingRoleID();
            enumList.Insert(0, SysCategory.ALL);
            foreach (var item in enumList)
            {
                Log2Net.Models.SysCategory curEnum = item;
                var appName = Configuration.SysNameMap.GetMySystemName(curEnum);
                tar.Add(new SelectListItem() { Text = appName + "[id=" + ((int)item).ToString() + "]", Value = ((int)item).ToString() });
            }
            return AbpJson(tar, null, null, JsonRequestBehavior.AllowGet, true, false);
        }

        //获取监控数据列表,table显示用
        public ActionResult GetLogMonitorData(MonitorSearchVM searchePara)
        {
            GetLogMonitorInput monitorIn = ConvertSearchVMToGetLogMonitorInput(searchePara);
            PagedResultDto<Log_SystemMonitorDto> monitors = _logMonitorService.GetLogMonitors(monitorIn);
            var result = Abp.AutoMapper.AutoMapExtensions.MapTo<PagedResultDto<Log_OperateMonitorView>>(monitors);// monitors.MapTo<PagedResultDto<Log_OperateMonitorView>>();
            result.Items = result.Items.Select(a =>
            {
                a.RunHours = Convert.ToDouble(a.RunHours.ToString("f2"));
                a.CpuUsage = Convert.ToDouble(a.CpuUsage.ToString("f2"));
                a.MemoryUsage = Convert.ToDouble(a.MemoryUsage.ToString("f2"));
                a.CurProcMem = Convert.ToDouble(a.CurProcMem.ToString("f2"));
                a.CurProcMemUse = Convert.ToDouble(a.CurProcMemUse.ToString("f2"));
                a.CurProcCpuUse = Convert.ToDouble(a.CurProcCpuUse.ToString("f2"));
                a.CurSubProcMem = Convert.ToDouble(a.CurSubProcMem.ToString("f2"));
                if (string.IsNullOrEmpty(a.DiskSpace)) { a.DiskSpace = null; return a; }
                a.DiskSpace = string.Join(";", SerializeHelper.XMLDESerializer<List<DiskSpaceEdm>>(a.DiskSpace).Select(p => p.DiscName.Trim(':') + ":" + p.Free + "G(" + p.Rate + "%)"));
                return a;
            }).ToList();
            var json = AbpJson(new { total = result.TotalCount, rows = result.Items }, null, null, JsonRequestBehavior.AllowGet, true, false);
            return json;
        }


        GetLogMonitorInput ConvertSearchVMToGetLogMonitorInput(MonitorSearchVM searchePara)
        {
            GetLogMonitorInput monitorIn = new GetLogMonitorInput()
            {
                MaxResultCount = searchePara.limit,
                SkipCount = searchePara.offset,
                Sorting = searchePara.sortby,
                IsDesc = searchePara.sortway == "asc" ? false : true,
                StartT = string.IsNullOrEmpty(searchePara.from) ? new DateTime() : Convert.ToDateTime(searchePara.from),
                EndT = string.IsNullOrEmpty(searchePara.to) ? new DateTime() : Convert.ToDateTime(searchePara.to).AddDays(1),
                Express = null,
                ServerHost = searchePara.host,
                ServerIP = "",
                SystemID = new List<SysCategory>() { searchePara.app },
                Other = searchePara.other
            };
            return monitorIn;
        }




    }
}