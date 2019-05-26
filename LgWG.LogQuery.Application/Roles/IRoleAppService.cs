using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.Roles.Dto;
using Log2Net.Models;

namespace LgWG.LogQuery.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
        List<SysCategory> GetSysCategoryAccordingRoleID();
    }
}
