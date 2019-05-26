using Abp.Domain.Repositories;
using LgWG.LogQuery.BasicRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogTrace
{
    public interface ILog_OperateTraceRepository : IBaseRepository<Log_OperateTrace, long> // IRepository<Log_OperateTrace, long>
    {

    }
}
