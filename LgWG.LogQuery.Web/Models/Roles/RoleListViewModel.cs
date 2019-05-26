using System.Collections.Generic;
using LgWG.LogQuery.Roles.Dto;
using Log2Net.Models;

namespace LgWG.LogQuery.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }

        public IReadOnlyList<SysCategory> SysCateIDs { get; set; }
    }
}