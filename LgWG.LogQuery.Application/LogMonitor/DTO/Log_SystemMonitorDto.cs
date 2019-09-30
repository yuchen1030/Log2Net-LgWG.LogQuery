using Abp.Application.Services.Dto;
using System;

namespace LgWG.LogQuery.LogMonitor.DTO
{
    public class Log_SystemMonitorDto : EntityDto
    {
        public DateTime LogTime { get; set; }


        public Log2Net.Models.SysCategory SystemID { get; set; }


        public string ServerHost { get; set; }


        public string ServerIP { get; set; }


        public int OnlineCnt { get; set; }


        public int AllVisitors { get; set; }

        public double RunHours { get; set; }


        public double CpuUsage { get; set; }


        public double MemoryUsage { get; set; }


        public int ProcessNum { get; set; }


        public int ThreadNum { get; set; }


        public int CurProcThreadNum { get; set; }

        public double CurProcMem { get; set; }


        public double CurProcMemUse { get; set; }


        public double CurProcCpuUse { get; set; }


        public double CurSubProcMem { get; set; }


        public string PageViewNum { get; set; }

        public string DiskSpace { get; set; }

        public string Remark { get; set; }
    }
}
