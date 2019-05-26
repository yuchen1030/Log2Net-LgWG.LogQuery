using System.Threading.Tasks;
using Abp.Application.Services;
using LgWG.LogQuery.Sessions.Dto;

namespace LgWG.LogQuery.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
