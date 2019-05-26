using Abp.Authorization;
using LgWG.LogQuery.Authorization.Roles;
using LgWG.LogQuery.Authorization.Users;

namespace LgWG.LogQuery.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
