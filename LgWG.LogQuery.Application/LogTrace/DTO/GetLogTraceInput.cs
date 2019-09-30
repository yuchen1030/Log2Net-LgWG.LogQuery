using LgWG.LogQuery.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogTrace.DTO
{
    public class GetLogTraceInput : LogSearchInput<Log_OperateTrace>
    {
        public Log2Net.Models.LogType LogType { get; set; }
        public string UserName { get; set; }
        public string ModuTable { get; set; }

        public string KeyWord { get; set; }
        public GetLogTraceInput()
        {
            Express = a =>    a.LogTime.Year >= 2018;
        }

    }
}
