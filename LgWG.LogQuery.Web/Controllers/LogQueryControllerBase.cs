using Abp.Application.Services.Dto;
using Abp.IdentityFramework;
using Abp.UI;
using Abp.Web.Mvc.Controllers;
using LgWG.LogQuery.Users;
using LgWG.LogQuery.Web.Models;
using Log2Net;
using Log2Net.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using LgWG.LogQuery.Users.Dto;
using System.Threading.Tasks;

namespace LgWG.LogQuery.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class LogQueryControllerBase : AbpController
    {
        private readonly IUserAppService _userAppService;
        protected LogQueryControllerBase(IUserAppService userAppService = null)
        {
            LocalizationSourceName = LogQueryConsts.LocalizationSourceName;
            _userAppService = userAppService;
        }

        protected virtual void CheckModelState()
        {
            if (!ModelState.IsValid)
            {
                throw new UserFriendlyException(L("FormIsNotValidMessage"));
            }
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }


        protected string WriteLog(LogLevel logLevel, params LogTraceVM[] model)
        {
            var myLogModels = model.Select(a => new LogTraceEdm()
            {
                UserId = GetCurUserID(),
                UserName = GetCurUserName(),
                Detail = a.Detail,
                LogType = a.LogType,
                Remark = a.Remark,
                TabOrModu = a.TabOrModu,
            }).ToArray();

            return LogApi.WriteLog(LogLevel.Info, myLogModels);
        }

        string GetCurUserID()
        {
            var userID = AbpSession.UserId ?? -1;
            var userIDStr = userID == -1 ? "" : userID.ToString();
            return userIDStr;
        }

        string GetCurUserName()
        {
            var userID = AbpSession.UserId ?? -1;
            var userIDStr = userID == -1 ? "" : userID.ToString();
            var userName = userIDStr;
            try
            {
                var user = Task.Run(async () => { return await _userAppService.Get(new EntityDto<long>() { Id = userID }); }).Result;
                if (user != null)
                {
                    userName = user.UserName;
                }
                return userName;
            }
            catch (Exception ex)
            {
                return "系统";
            }



        }



    }
}