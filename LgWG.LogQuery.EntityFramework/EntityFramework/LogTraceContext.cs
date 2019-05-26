namespace LgWG.LogQuery.EntityFramework
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using LgWG.LogQuery.LogTrace;
    using Abp.Zero.EntityFramework;
    using LgWG.LogQuery.MultiTenancy;
    using LgWG.LogQuery.Authorization.Roles;
    using LgWG.LogQuery.Authorization.Users;
    using System.Data.Common;
    using Abp.EntityFramework;

    public class LogTraceContext : AbpDbContext //DbContext  £¬ AbpDbContext  £¬   AbpZeroDbContext<Tenant, Role, User>
    {
        public LogTraceContext() : base("name=LogTraceContext")
        {
        }


        public virtual DbSet<Log_OperateTrace> Log_OperateTrace { get; set; }


    }
}
