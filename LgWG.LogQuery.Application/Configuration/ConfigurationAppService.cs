using Abp.Authorization;
using Abp.Runtime.Session;
using LgWG.LogQuery.Configuration.Dto;
using System.Threading.Tasks;

namespace LgWG.LogQuery.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : LogQueryAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
