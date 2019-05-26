using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LgWG.LogQuery.DTO
{
    public class LogSearchInput<T> : SearchBaseInputDto where T : class, new()
    {
        public DateTime StartT { get; set; }
        public DateTime EndT { get; set; }
        public List<Log2Net.Models.SysCategory> SystemID { get; set; }
        public string ServerHost { get; set; }
        public string ServerIP { get; set; }
        public Expression<Func<T, bool>> Express { get; set; }

        public LogSearchInput()
        {
            Sorting = "LogTime";
        }
    }
}
