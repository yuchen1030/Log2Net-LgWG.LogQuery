using System.Collections.Generic;
using LgWG.LogQuery.Roles.Dto;
using LgWG.LogQuery.Users.Dto;

namespace LgWG.LogQuery.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}