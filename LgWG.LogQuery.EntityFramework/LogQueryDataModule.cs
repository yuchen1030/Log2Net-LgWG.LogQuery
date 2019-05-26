using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using LgWG.LogQuery.EntityFramework;

namespace LgWG.LogQuery
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(LogQueryCoreModule))]
    public class LogQueryDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<LogQueryDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
