using Abp.Application.Services.Dto;
using System;

namespace LgWG.LogQuery.LogTrace.DTO
{
    public class Log_OperateTraceDto : EntityDto
    {

        public DateTime LogTime { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public Log2Net.Models.LogType LogType { get; set; }
        public Log2Net.Models.SysCategory SystemID { get; set; }
        public string ServerHost { get; set; }
        public string ServerIP { get; set; }
        public string ClientHost { get; set; }
        public string ClientIP { get; set; }
        public string TabOrModu { get; set; }
        public string Detail { get; set; }   
        public string Remark { get; set; }
    }

    public class NameData
    {
        public string Title { get; set; }
        public string[] Name { get; set; }
        public int[] Data { get; set; }
    }

}
