using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace LgWG.LogQuery.MigrationsLogTrace
{
    internal sealed class Configuration : DbMigrationsConfiguration<LgWG.LogQuery.EntityFramework.LogTraceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"MigrationsLogTrace";
        }

        protected override void Seed(EntityFramework.LogTraceContext context)
        {
            //context.Courses.AddOrUpdate(c => c.CourseName,
            //    new Course("Mathematics"),
            //    new Course("Physics")
            //    );
        }
    }
}
