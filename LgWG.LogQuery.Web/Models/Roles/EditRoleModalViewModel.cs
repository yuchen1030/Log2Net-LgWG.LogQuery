using System.Collections.Generic;
using System.Linq;
using LgWG.LogQuery.Roles.Dto;
using Log2Net.Models;

namespace LgWG.LogQuery.Web.Models.Roles
{
    public class EditRoleModalViewModel
    {
        public RoleDto Role { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }

        public bool HasPermission(PermissionDto permission)
        {
            return Permissions != null && Role.Permissions.Any(p => p == permission.Name);
        }

        public IReadOnlyList<SysCategory> SysCateIDs { get; set; }


        public bool HasSysCategory(SysCategory permission)
        {
            var sysIDs = string.Join("", Role.SysCateIDs).Split(',');
            if (sysIDs.Contains(((int)SysCategory.ALL).ToString()))
            {
                return true;
            }
            return SysCateIDs != null && sysIDs.Any(p => p == ((int)(permission)).ToString());
        }

    }
}