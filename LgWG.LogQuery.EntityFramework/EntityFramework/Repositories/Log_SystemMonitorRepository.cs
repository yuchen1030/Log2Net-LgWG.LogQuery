using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;
using LgWG.LogQuery.LogMonitor;
using System.Collections.Generic;
using System.Linq;

namespace LgWG.LogQuery.EntityFramework.Repositories
{

    public class Log_SystemMonitorRepository : BaseRepository<LogMonitorContext, Log_SystemMonitor, long>, ILog_SystemMonitorRepository
    {
        public Log_SystemMonitorRepository(IDbContextProvider<LogMonitorContext> dbContextProvider) : base(dbContextProvider)
        {
        }

    }



}
