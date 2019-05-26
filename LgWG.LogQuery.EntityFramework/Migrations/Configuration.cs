using System.Data.Entity.Migrations;
using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using LgWG.LogQuery.Migrations.SeedData;
using EntityFramework.DynamicFilters;

namespace LgWG.LogQuery.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<LogQuery.EntityFramework.LogQueryDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LogQuery";
        }

        protected override void Seed(LogQuery.EntityFramework.LogQueryDbContext context)
        {
            context.DisableAllFilters();

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                new TenantRoleAndUserBuilder(context, 1).Create();
            }
            else
            {
                //You can add seed for tenant databases and use Tenant property...
            }

            context.SaveChanges();
        }
    }
}
