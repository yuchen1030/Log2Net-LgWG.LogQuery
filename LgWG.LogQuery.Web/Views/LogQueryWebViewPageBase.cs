using Abp.Web.Mvc.Views;

namespace LgWG.LogQuery.Web.Views
{
    public abstract class LogQueryWebViewPageBase : LogQueryWebViewPageBase<dynamic>
    {

    }

    public abstract class LogQueryWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected LogQueryWebViewPageBase()
        {
            LocalizationSourceName = LogQueryConsts.LocalizationSourceName;
        }
    }
}