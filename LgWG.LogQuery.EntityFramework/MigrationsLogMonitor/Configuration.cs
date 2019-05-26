using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.MigrationsLogMonitor
{
    internal sealed class Configuration : DbMigrationsConfiguration<LgWG.LogQuery.EntityFramework.LogMonitorContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"MigrationsLogMonitor";
        }

        protected override void Seed(EntityFramework.LogMonitorContext context)
        {
            //context.Courses.AddOrUpdate(c => c.CourseName,
            //    new Course("Mathematics"),
            //    new Course("Physics")
            //    );
        }
    }
}
