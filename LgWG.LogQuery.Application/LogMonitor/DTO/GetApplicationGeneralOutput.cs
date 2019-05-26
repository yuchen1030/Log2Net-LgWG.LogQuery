using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.LogMonitor.DTO
{

    public class ApplicationData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IList<ApplicationHost> Hosts { get; set; }
    }

    [AutoMapFrom(typeof(Log_SystemMonitor))]
    public class ApplicationHost
    {
        public string ID { get; set; }
        public bool Enabled { get { return true; } set { value = true; } }
        public string ServerHost { get; set; }
        public decimal CpuUsage { get; set; }
        public decimal MemoryUsage { get; set; }
        public DateTime Time { get; set; }
    }

}
