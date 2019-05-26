using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace LgWG.LogQuery.Authorization
{
    public class LogQueryAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            context.CreatePermission(PermissionNames.Pages_LogTrace, L("LogTrace"));
            context.CreatePermission(PermissionNames.Pages_LogMonitor, L("LogMonitor"));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LogQueryConsts.LocalizationSourceName);
        }
    }
}
