using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Web.Mvc.Authorization;
using LgWG.LogQuery.Authorization;
using LgWG.LogQuery.LogMonitor;
using LgWG.LogQuery.LogMonitor.DTO;
using LgWG.LogQuery.LogTrace;
using LgWG.LogQuery.LogTrace.DTO;
using LgWG.LogQuery.Users;
using LgWG.LogQuery.Web.Models;
using Log2Net;
using Log2Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace LgWG.LogQuery.Web.Controllers
{
    [AbpMvcAuthorize]
    [AbpMvcAuthorize(PermissionNames.Pages_LogTrace)]
    public class LogTraceController : LogQueryControllerBase
    {
        // GET: LogTrace

        private readonly ILog_OperateTraceService _logTraceService;
        private readonly ILog_SystemMonitorService _logMonitorService;
        private readonly IUserAppService _userAppService;

        public LogTraceController(ILog_OperateTraceService logTraceService, ILog_SystemMonitorService logMonitorService, IUserAppService userAppService) : base(userAppService)
        {
            _logTraceService = logTraceService;
            _logMonitorService = logMonitorService;
            _userAppService = userAppService;
        }

        public async Task<ActionResult> Index()
        {
            LgWG.LogQuery.Web.Models.IndexView iv = new Models.IndexView();
            iv.Apps = GetApply(DateTime.Now.AddDays(-7).ToString(), DateTime.Now.ToString());
            LogTraceVM log = new LogTraceVM() { LogType = LogType.业务记录, TabOrModu = "操作日志", Detail = "进入了页面" };
            //string msg = WriteLog(LogLevel.Info, log);
            return View(iv);
        }

        //获取服务器列表及其状态概况
        public ActionResult GetAppStatus(string startT, string endT)
        {
            IList<ApplicationData> result = GetApply(startT, endT);
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Content(data);
        }

        //获取轨迹日志数据
        public ActionResult GetLogTraceData(TraceSearchVM para)
        {
            GetLogTraceInput traceIn = new GetLogTraceInput()
            {
                MaxResultCount = para.limit,
                SkipCount = para.offset,
                Sorting = para.sortby,
                IsDesc = para.sortway == "asc" ? false : true,
                StartT = string.IsNullOrEmpty(para.from) ? new DateTime() : Convert.ToDateTime(para.from),
                EndT = string.IsNullOrEmpty(para.to) ? new DateTime() : Convert.ToDateTime(para.to).AddDays(1),
                Express = null,
                LogType = para.type,
                ServerHost = para.host,
                ServerIP = "",
                SystemID = new List<SysCategory>() { para.app },
                KeyWord = !string.IsNullOrEmpty(para.keyWord) ? para.keyWord.Trim() : "",
                UserName = !string.IsNullOrEmpty(para.userName) ? para.userName.Trim() : "",
                ModuTable = !string.IsNullOrEmpty(para.modTab) ? para.modTab.Trim() : "",

            };

            GetLogTraceOutput traces = _logTraceService.GetLogTraces(traceIn);

            try
            {
                traces.Items = traces.Items.Select(a => { a.ClientHost = a.ClientHost.Replace(".LgWG.com", ""); return a; }).ToList();
            }
            catch
            {

            }

            var resultItems = traces.Items.MapTo<List<Log_OperateTraceView>>();
            var errLogNum = 8;
            var json = AbpJson(new { total = traces.TotalCount, rows = resultItems, ErrLogNum = traces.ErrLogNum, NameDatas = traces.NameDatas }, null, null, JsonRequestBehavior.AllowGet, true, false);
            return json;
        }


        //获取所有的日志类型的下拉列表
        public ActionResult GetAllLogTypeList()
        {
            List<SelectListItem> tar = new List<SelectListItem>();
            var dic = ComClass.GetDicFromEnumType(new LogType());
            try
            {
                dic.Add(LogType.所有.ToString(), ((int)LogType.所有));
            }
            catch
            {
            }
            dic = dic.OrderBy(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
            foreach (var item in dic)
            {
                tar.Add(new SelectListItem() { Text = item.Key, Value = item.Value.ToString() });
            }



            return AbpJson(tar, null, null, JsonRequestBehavior.AllowGet, true, false);
        }


        List<ApplicationData> GetApply(string startT, string endT)
        {
            var apps = _logMonitorService.GetLatestApplicationGeneral(startT, endT);
            return apps;
        }


    }
}