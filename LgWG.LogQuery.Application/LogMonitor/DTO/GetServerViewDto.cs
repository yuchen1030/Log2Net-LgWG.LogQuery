using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogMonitor.DTO
{

    public class LogMonitorVM
    {
        public List<string> WebSites { get; set; }
        public List<string> Servers { get; set; }
        public int LogNum { get; set; }
        public string LogTime { get; set; }
        public List<ServersViewDto> ServerData { get; set; }

    }

    public class ServersViewDto
    {
        public string ServerName { get; set; }
        public string AppName { get; set; }
        public List<ServerViewDto> Monitors { get; set; }
    }

    public class ServerViewDto
    {
        public string LogTime { get; set; }
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public int CurProcThreadNum { get; set; }
        public int OnlineCnt { get; set; }
        public string DiskSpace { get; set; }//磁盘使用情况 

    }


}
