using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.Roles.Dto;
using LgWG.LogQuery.Users.Dto;

namespace LgWG.LogQuery.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}