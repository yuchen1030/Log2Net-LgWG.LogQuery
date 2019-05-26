using LgWG.LogQuery.LogMonitor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using LgWG.LogQuery.Authorization;
using Abp.Domain.Repositories;
using Abp.Authorization.Users;
using LgWG.LogQuery.Authorization.Roles;

using Log2Net;
using Abp.Runtime.Caching;
using Log2Net.Models;
using Log2Net.Util;

namespace LgWG.LogQuery.LogMonitor
{

    /// <summary>
    /// 系统监控接口的实现
    /// </summary>
  //  [AbpAuthorize]
    [AbpAllowAnonymous]
    // [AbpAuthorize(PermissionNames.Pages_LogMonitor)]
    public class Log_SystemMonitorService : LogQueryAppServiceBase, ILog_SystemMonitorService
    {
        private readonly ILog_SystemMonitorRepository _logMonitorRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly ICacheManager _cacheManager;
        //  private readonly IRepository<Log_SystemMonitor, long> _logMonitorRepository2;//也可不定义ILog_SystemMonitorRepository，直接使用此写法

        /// <summary>
        /// 构造函数自动注入
        /// </summary>
        /// <param name="logMonitorRepository"></param>
        public Log_SystemMonitorService(ILog_SystemMonitorRepository logMonitorRepository, IRepository<UserRole, long> userRoleRepository, IRepository<Role> roleRepository, ICacheManager cacheManager) : base(userRoleRepository, roleRepository, cacheManager)
        {
            _logMonitorRepository = logMonitorRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// 获取监控记录（具体实现）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResultDto<Log_SystemMonitorDto> GetLogMonitors(GetLogMonitorInput input)
        {
            var query = GetLogMonitorIQueryableData(input);
            var totalCount = query.Count();
            var list = query.PageBy(input).ToList();
            var listDtos = list.MapTo<List<Log_SystemMonitorDto>>();
            return new PagedResultDto<Log_SystemMonitorDto>(totalCount, listDtos);
        }


        /// <summary>
        /// 获取监控记录（每个服务器各获取指定的条数）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<ServersViewDto> GetLogMonitorsEveryServer(GetLogMonitorInput input)
        {
            var other = (!string.IsNullOrEmpty(input.Other) ? input.Other : "").Split(';'); //input.Other包含时间格式和点数的范围，用;隔开
            string chartTimeFormate = other[0];
            int dotRange = other.Length < 2 ? 10000 : ((int)Convert.ToDouble(other[1]));
            dotRange = dotRange < 0 ? -dotRange : dotRange;
            dotRange = dotRange > 10000 ? 10000 : dotRange;//最多1000点
            try
            {
                var query = GetLogMonitorIQueryableData(input).OrderByDescending(a => a.LogTime).ToList();
                var groups = (from d in query
                              group d by new {  /* d.SystemID,*/ d.ServerHost } into g
                              select new
                              {
                                  Name = g.Key,//  SysID = g.Key.SystemID,
                                  HostID = g.Key.ServerHost,
                                  count = g.Count(),
                                  // datas = g.OrderByDescending(a => a.Time).Take(input.MaxResultCount),//只能取到最新的数据，即使指定范围内有更多的数据
                                  datas = g.OrderByDescending(a => a.LogTime).Take(dotRange).Select((log, index) => SelectSpecialData(index, log, Math.Min(g.Count(), dotRange), input.MaxResultCount
                                   , GetDataByInterval(Math.Min(g.Count(), dotRange), input.MaxResultCount))).Where(a => a != null),//指定范围内间隔相等的取出数据
                              }).ToList();

                List<ServersViewDto> sers = new List<ServersViewDto>();
                foreach (var item in groups)
                {
                    var key = item.HostID;
                    var value = item.datas.OrderBy(a => a.LogTime).ToList();
                    var apps = value.Select(a => a.SystemID).Distinct().ToList();
                    List<ServerViewDto> Monitors = value.MapTo<List<ServerViewDto>>().ToList();
                    Monitors = Monitors.Select(a => { a.LogTime = Convert.ToDateTime(a.LogTime).ToString(chartTimeFormate); return a; }).ToList();
                    ServersViewDto curServer = new ServersViewDto() { ServerName = key, AppName = string.Join(",", apps), Monitors = Monitors };
                    sers.Add(curServer);
                }
                return sers;
            }
            catch
            {
                return new List<ServersViewDto>();
            }
        }




        /// <summary>
        /// 获取各个服务器指定范围内的数据，用于曲线显示
        /// </summary>
        /// <param name="range"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public LogMonitorVM GetMonitorChartData(string range, long userId)
        {
            int limit = 1;
            string startT = "", endT = "";
            string other = "";
            var ttt = AbpSession.UserId;
            string timeFormate = "";
            if (!string.IsNullOrEmpty(range))
            {
                //range格式 ：模式[_数值1][_数值2]_点数_时间格式，最后两个为点数和时间格式。例如  3_100_100_M:d H:m:s
                //模式0：自定义时段，两个数值为时间点
                //模式1：当天
                //模式2：最近xx小时
                //模式3：最新xx点
                var temp = range.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                timeFormate = temp[temp.Length - 1];//时间格式
                other = timeFormate;
                if (temp.Length != 1)
                {
                    limit = (int)(Convert.ToDouble(temp[temp.Length - 2]));
                    var mode = temp[0];

                    if (mode == "0")
                    {
                        startT = temp[1];
                        endT = temp[2];
                    }
                    else if (mode == "1")
                    {
                        startT = DateTime.Now.ToString("yyyy-MM-dd");
                        endT = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                    }
                    else if (mode == "2")
                    {
                        startT = DateTime.Now.AddHours(-1 * Convert.ToDouble(temp[1])).ToString("yyyy-M-d H:m:s");
                        endT = DateTime.Now.ToString();
                    }
                    else if (mode == "3")
                    {
                        other += ";" + temp[1];
                    }

                }
                else  //只获取最新的一条数据
                {

                }
            }
            var enumList = GetUserSysCategoryAccordingRoleID(userId);
            GetLogMonitorInput monitorIn = new GetLogMonitorInput()
            {
                MaxResultCount = limit,
                SkipCount = 0,//searchePara.offset,
                Sorting = "LogTime",
                IsDesc = true,
                StartT = string.IsNullOrEmpty(startT) ? new DateTime() : Convert.ToDateTime(startT),
                EndT = string.IsNullOrEmpty(endT) ? new DateTime() : Convert.ToDateTime(endT),
                Express = null,
                ServerHost = "",  //应为空，V-WEBAPI为测试
                ServerIP = "",
                SystemID = enumList,
                Other = other
            };
            List<ServersViewDto> sers = GetLogMonitorsEveryServer(monitorIn);
            //if (limit == 1) //测试增加一点的情况，改写数据便于看的清除
            //{
            //    sers = sers.Select(a => { a.Monitors = a.Monitors.Select(m => { m.Time = DateTime.Now.ToString(timeFormate); m.CpuUsage += 5; m.MemoryUsage += 6; m.CurProcThreadNum += 7; m.OnlineCnt += 8; return m; }).ToList(); return a; }).ToList();//测试
            //}
            LogMonitorVM vm = new LogMonitorVM()
            {
                ServerData = sers,        
                WebSites = enumList.Select(a => Configuration.SysNameMap.GetMySystemName(a)).ToList(),
                Servers = GetAllServerList(),
                LogNum = GetTodayMonitorLogNum(enumList),
                LogTime = DateTime.Now.ToString("yyyy-M-d H:m:s")
            };
            return vm;
        }


        /// <summary>
        /// 获取最新的网站概况信息（具体实现）
        /// </summary>
        /// <returns></returns>
        public List<ApplicationData> GetLatestApplicationGeneral(string startT, string endT)
        {
            bool bTimeOK = false;
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            try
            {
                dtStart = Convert.ToDateTime(startT);
                dtEnd = Convert.ToDateTime(endT);
                bTimeOK = true;
            }
            catch
            {

            }
            var enumList = GetUserSysCategoryAccordingRoleID();
            var query = (from d in _logMonitorRepository.GetAll()
                         where enumList.Contains(d.SystemID)
                         group d by new { d.SystemID, d.ServerHost } into g
                         select new
                         {
                             Time = g.Max(x => x.LogTime),
                             Name = g.Key,
                             SysID = g.Key.SystemID,
                             HostID = g.Key.ServerHost,
                             datas = g.OrderByDescending(a => a.LogTime).FirstOrDefault()
                         }).ToList();
            var applications = query.GroupBy(a => a.SysID);

            List<ApplicationData> apps = new List<ApplicationData>();

            foreach (var item in applications)
            {
                var key = item.Key;
                var value = item.Select(a => a.datas).ToList();
                var hosts = value.MapTo<List<ApplicationHost>>();
                if (bTimeOK)
                {
                    //hosts.Select(a => { if (a.Time >= dtStart && a.Time < dtEnd) { a.Enabled = true; } return a; }).ToList();
                }
                if (hosts.Count > 0)
                {
                    apps.Add(new ApplicationData() { ID = ((int)key).ToString(), Name = Configuration.SysNameMap.GetMySystemName(key), Hosts = hosts });
                }
            }
            return apps;
        }

        /// <summary>
        /// 获取所有的服务器列表信息(具体实现)
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllServerList()
        {
            var list = _logMonitorRepository.GetAll().Select(a => a.ServerHost).Distinct().ToList();
            return list;
        }

        /// <summary>
        /// 今日监控日志数量
        /// </summary>
        /// <returns></returns>
        public int GetTodayMonitorLogNum(List<SysCategory> sysCategoryList)
        {
            GetLogMonitorInput input = new GetLogMonitorInput() { SystemID = sysCategoryList, StartT = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), EndT = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")) };
            var query = GetLogMonitorIQueryableData(input);
            return query.Count();
        }

        //根据检索条件获取数据，并排序（未分页）
        IQueryable<Log_SystemMonitor> GetLogMonitorIQueryableData(GetLogMonitorInput input)
        {
            var enumList = GetUserSysCategoryAccordingRoleID();
            var query = _logMonitorRepository.GetAll();
            query = query.WhereIf(input.StartT != new DateTime(), a =>  a.LogTime>= input.StartT & a.LogTime < input.EndT);
            if (!input.SystemID.Contains(SysCategory.ALL))
            {
                query = query.Where(a => input.SystemID.Contains(a.SystemID));
            }
            else
            {
                query = query.Where(a => enumList.Contains(a.SystemID));
            }
            query = query.WhereIf(!string.IsNullOrEmpty(input.ServerHost), a => a.ServerHost == input.ServerHost);
            query = query.WhereIf(!string.IsNullOrEmpty(input.ServerIP), a => a.ServerIP == input.ServerIP);
            query = query.WhereIf(input.Express != null, input.Express);
            input.Sorting = input.Sorting.Replace("SysName", "SystemID");
            query = query.OrderBy(input.Sorting);
            return query;
        }


        //在total个数据中，取出tarNum个数据（尽量做到间隔相等）
        Log_SystemMonitor SelectSpecialData(int index, Log_SystemMonitor log, int total, int tarNum, List<int> tarIndexs)
        {
            if (total <= tarNum)
            {
                return log;
            }
            if (tarIndexs.Contains(index))
            {
                return log;
            }
            return null;
        }


        //从0~total中取出tarNum个数,尽量做到间隔相等
        List<int> GetDataByInterval(int total, int tarNum)
        {
            int[] tarArr = new int[tarNum];
            List<int> tarList = new List<int>();
            int d = (total - 1) / tarNum;
            if (d <= 0)
            {
                return tarList;
            }
            if ((d + 1) * (tarNum - 1) <= total)   //等差数列
            {
                for (int i = 1; i < tarNum; i++)
                {
                    tarList.Add((i - 1) * (d + 1));
                }
            }
            else
            {
                tarList.Add(0);

                if (d == 1 && total % tarNum < tarNum / 2.0)  //例如total = 10，tarNum=7，随机数取值法
                {
                    Random r = new Random();
                    //首先随机取出需要排除的 total -  tarNum个元素
                    List<int> exList = new List<int>();
                    while (exList.Count < total - tarNum)
                    {
                        int cur = r.Next(1, total - 1);
                        if (!exList.Contains(cur))
                        {
                            exList.Add(cur);
                        }
                    }
                    //再从1-total中获得除排除掉之外的元素
                    for (int i = 1; i < total - 1; i++)
                    {
                        if (!exList.Contains(i))
                        {
                            tarList.Add(i);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < tarNum; i++)   //类等差数列，差在 d ，d+1之间变化
                    {
                        int d1 = (i - 1) % 2 == 0 ? d + 1 : d;

                        tarList.Add(tarList[i - 1] + d1);
                    }
                }
            }

            if (tarList.Count == tarNum)
            {
                tarList.RemoveAt(tarNum - 1);
            }
            else if (tarList.Count > tarNum)
            {
                tarList = tarList.Take(tarNum - 1).ToList();
            }
            else if (tarList.Count < tarNum - 1)
            {

            }
            tarList.Sort();
            tarList.Add(total - 1);

            tarList = tarList.Select(a => total - 1 - a).ToList();

            return tarList;
        }





    }

}
