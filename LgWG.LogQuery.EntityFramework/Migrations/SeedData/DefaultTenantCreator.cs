using System.Linq;
using LgWG.LogQuery.EntityFramework;
using LgWG.LogQuery.MultiTenancy;

namespace LgWG.LogQuery.Migrations.SeedData
{
    public class DefaultTenantCreator
    {
        private readonly LogQueryDbContext _context;

        public DefaultTenantCreator(LogQueryDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserAndRoles();
        }

        private void CreateUserAndRoles()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                _context.Tenants.Add(new Tenant {TenancyName = Tenant.DefaultTenantName, Name = Tenant.DefaultTenantName});
                _context.SaveChanges();
            }
        }
    }
}
