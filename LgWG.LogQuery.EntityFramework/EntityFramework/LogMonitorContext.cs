namespace LgWG.LogQuery.EntityFramework
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using LgWG.LogQuery.LogMonitor;
    using Abp.EntityFramework;

    public partial class LogMonitorContext : AbpDbContext//DbContext  £¬ AbpDbContext  £¬   AbpZeroDbContext<Tenant, Role, User>
    {
        public LogMonitorContext() : base("name=LogMonitorContext")
        {
        }

        public virtual DbSet<Log_SystemMonitor> Log_SystemMonitor { get; set; }


    }
}
