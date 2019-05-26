using System.Threading.Tasks;
using Abp.Application.Services;
using LgWG.LogQuery.Authorization.Accounts.Dto;

namespace LgWG.LogQuery.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
