using Abp.Application.Services;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using LgWG.LogQuery.Authorization.Roles;
using LgWG.LogQuery.Authorization.Users;
using LgWG.LogQuery.MultiTenancy;
using LgWG.LogQuery.Users.Dto;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LgWG.LogQuery
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class LogQueryAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }


        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly ICacheManager _cacheManager;
        protected LogQueryAppServiceBase()
        {
            LocalizationSourceName = LogQueryConsts.LocalizationSourceName;
        }


        public LogQueryAppServiceBase(IRepository<UserRole, long> userRoleRepository, IRepository<Role> roleRepository, ICacheManager cacheManager)
        {
            LocalizationSourceName = LogQueryConsts.LocalizationSourceName;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _cacheManager = cacheManager;
        }


        //public class UserSysCategorys
        //{
        //    public long UserID { get; set; }
        //    public string SysCateIDs { get; set; }
        //}

        List<UserSysCategorys> GetSysCategoryForAllFromDB()
        {
            var sysCateIDs = (from userRole in _userRoleRepository.GetAll()
                              join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                              select new UserSysCategorys { UserID = userRole.UserId, SysCateIDs = role.SysCateIDs }
                              ).ToList();
            return sysCateIDs;
        }


        public List<Log2Net.Models.SysCategory> GetUserSysCategoryAccordingRoleID(long userId =-1)
        {

            var sysCateIDs = "";
            try
            {
                var allUsersSysCategory = _cacheManager.GetCache("LgWG.LogQuery").Get("UserSysCategorys", () => GetSysCategoryForAllFromDB()) as List<UserSysCategorys>;
                sysCateIDs = allUsersSysCategory.Where(a => 
               ( AbpSession.UserId != null && (long)a.UserID == (long)AbpSession.UserId)
               || ((long)a.UserID == userId)

                ).FirstOrDefault().SysCateIDs;
            }
            catch
            {
                sysCateIDs = (from userRole in _userRoleRepository.GetAll()
                              join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                              where (userRole.UserId == AbpSession.UserId   || userRole.UserId == userId || AbpSession.UserId == null)
                              select role.SysCateIDs).FirstOrDefault();
            }

            List<Log2Net.Models.SysCategory> sysList = new List<Log2Net.Models.SysCategory>();
            var ids = sysCateIDs.Split(',');
            if (ids.Contains(((int)(Log2Net.Models.SysCategory.ALL)).ToString()))
            {
                sysList = ComClass.GetMySysCategory();
                sysList.Remove(Log2Net.Models.SysCategory.ALL);
                return sysList;
            }
            else
            {
                foreach (var item in ids)
                {
                    Log2Net.Models.SysCategory curEnum = (Log2Net.Models.SysCategory)Convert.ToInt32(item);
                    sysList.Add(curEnum);
                }
            }
            return sysList;
        }



        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }




    }
}