using Abp.Runtime.Validation;
using LgWG.LogQuery.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogMonitor.DTO
{
    public class GetLogMonitorInput : LogSearchInput<Log_SystemMonitor> 
    {
        public GetLogMonitorInput()
        {
            Express = a => a.OnlineCnt >= 0;
        }

    }
}
