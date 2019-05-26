using System.Threading.Tasks;
using Abp.Application.Services;
using LgWG.LogQuery.Configuration.Dto;

namespace LgWG.LogQuery.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}