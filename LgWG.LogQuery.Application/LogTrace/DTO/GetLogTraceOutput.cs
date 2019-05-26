using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogTrace.DTO
{
    public class GetLogTraceOutput
    {
        public List<Log_OperateTraceDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int ErrLogNum { get; set; }
        public List<NameData> NameDatas { get; set; }
    }
}
