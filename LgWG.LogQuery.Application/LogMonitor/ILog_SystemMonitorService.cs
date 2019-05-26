using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.BasicRepository;
using LgWG.LogQuery.DTO;
using LgWG.LogQuery.LogMonitor.DTO;
using LgWG.LogQuery.LogTrace.DTO;
using Log2Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LgWG.LogQuery.LogMonitor
{
    /// <summary>
    /// 系统监控接口
    /// </summary>
    public interface ILog_SystemMonitorService : IApplicationService
    {
        /// <summary>
        /// 获取监控记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        PagedResultDto<Log_SystemMonitorDto> GetLogMonitors(GetLogMonitorInput input);

        /// <summary>
        /// 获取监控记录（每个服务器各获取指定的条数）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<ServersViewDto> GetLogMonitorsEveryServer(GetLogMonitorInput input);

        /// <summary>
        /// 获取各个服务器指定范围内的数据，用于曲线显示
        /// </summary>
        /// <param name="range"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        LogMonitorVM GetMonitorChartData(string range, long userId);

        /// <summary>
        /// 获取最新的网站概况信息
        /// </summary>
        /// <returns></returns>
        List<ApplicationData> GetLatestApplicationGeneral(string startT, string endT);

        /// <summary>
        /// 获取所有的服务器列表信息
        /// </summary>
        /// <returns></returns>
        List<string> GetAllServerList();


        /// <summary>
        /// 获取今日监控日志数量
        /// </summary>
        /// <returns></returns>
        int GetTodayMonitorLogNum(List<SysCategory> sysCategoryList);

    }


}
