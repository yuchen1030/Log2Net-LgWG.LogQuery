using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;
using LgWG.LogQuery.LogTrace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LgWG.LogQuery.EntityFramework.Repositories
{

    public class Log_OperateTraceRepository : BaseRepository<LogTraceContext, Log_OperateTrace, long>, ILog_OperateTraceRepository
    {
        public Log_OperateTraceRepository(IDbContextProvider<LogTraceContext> dbContextProvider) : base(dbContextProvider)
        {

        }

    }


}
