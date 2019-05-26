using System.Data.Common;
using Abp.Zero.EntityFramework;
using LgWG.LogQuery.Authorization.Roles;
using LgWG.LogQuery.Authorization.Users;
using LgWG.LogQuery.MultiTenancy;

namespace LgWG.LogQuery.EntityFramework
{
    public class LogQueryDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public LogQueryDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in LogQueryDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of LogQueryDbContext since ABP automatically handles it.
         */
        public LogQueryDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public LogQueryDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public LogQueryDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }
    }
}
