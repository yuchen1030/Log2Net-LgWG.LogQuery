using Abp.MultiTenancy;
using LgWG.LogQuery.Authorization.Users;

namespace LgWG.LogQuery.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}