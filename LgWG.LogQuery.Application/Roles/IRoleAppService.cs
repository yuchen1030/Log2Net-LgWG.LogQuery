using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.Roles.Dto;


namespace LgWG.LogQuery.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
        List<Log2Net.Models.SysCategory> GetSysCategoryAccordingRoleID();
    }
}
