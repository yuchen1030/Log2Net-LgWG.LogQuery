using Abp.AutoMapper;
using LgWG.LogQuery.Sessions.Dto;

namespace LgWG.LogQuery.Web.Models.Account
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}