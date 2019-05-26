using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LgWG.LogQuery.MultiTenancy.Dto;

namespace LgWG.LogQuery.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
