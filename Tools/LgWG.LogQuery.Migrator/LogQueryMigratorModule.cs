using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using LgWG.LogQuery.EntityFramework;

namespace LgWG.LogQuery.Migrator
{
    [DependsOn(typeof(LogQueryDataModule))]
    public class LogQueryMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<LogQueryDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}