using Abp.Application.Navigation;
using Abp.Localization;
using LgWG.LogQuery.Authorization;

namespace LgWG.LogQuery.Web
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See Views/Layout/_TopMenu.cshtml file to know how to render menu.
    /// </summary>
    public class LogQueryNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Home,
                        L("HomePage"),
                        url: "",
                        icon: "home",
                        requiresAuthentication: true
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.LogTrace,
                        L("LogTrace"),
                        url: "LogTrace/Index",
                        icon: "assignment",   //assessment为柱状图式图标，assignment为水平图表
                        requiredPermissionName: PermissionNames.Pages_LogTrace,
                        requiresAuthentication: true

                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.LogMonitor,
                        L("LogMonitor"),
                        url: "Home/Index", //LogMonitor/Index
                        icon: "important_devices",   //assessment为柱状图式图标，assignment为水平图表
                        requiresAuthentication: true,
                               requiredPermissionName: PermissionNames.Pages_LogMonitor
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        "UserRoleMgr",
                        L("UserRoleMgr"),
                        icon: "accessibility"
                    ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Tenants"),
                        url: "Tenants",
                        icon: "business",
                        requiredPermissionName: PermissionNames.Pages_Tenants
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Roles,
                        L("Roles"),
                        url: "Roles",
                        icon: "local_offer",
                        requiredPermissionName: PermissionNames.Pages_Roles
                    )
                ).AddItem(
                        new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "people",
                        requiredPermissionName: PermissionNames.Pages_Users
                    )
                )).AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("About"),
                        url: "About",
                        icon: "info"
                    )
                )
                ;
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LogQueryConsts.LocalizationSourceName);
        }
    }
}
