using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.LogTrace.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogTrace
{
    /// <summary>
    /// 轨迹日志接口
    /// </summary>
    public interface ILog_OperateTraceService : IApplicationService
    {
        /// <summary>
        /// 获取轨迹日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        GetLogTraceOutput GetLogTraces(GetLogTraceInput input);

    }
}
