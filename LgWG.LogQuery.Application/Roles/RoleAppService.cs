using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Runtime.Caching;
using Abp.UI;
using LgWG.LogQuery.Authorization.Roles;
using LgWG.LogQuery.Authorization.Users;
using LgWG.LogQuery.Roles.Dto;
using Log2Net.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LgWG.LogQuery.Roles
{
    [AbpAuthorize]
    public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly ICacheManager _cacheManager;
        public RoleAppService(
            IRepository<Role> repository,
            RoleManager roleManager,
            UserManager userManager,
            IRepository<User, long> userRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository, ICacheManager cacheManager)
            : base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _cacheManager = cacheManager;
        }

        public override async Task<RoleDto> Create(CreateRoleDto input)
        {
            CheckCreatePermission();

            var role = ObjectMapper.Map<Role>(input);
            role.SysCateIDs = string.Join(",", input.SysCateIDs);
            CheckErrors(await _roleManager.CreateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.Permissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
            ClearUserSysCategorysCache();
            return MapToEntityDto(role);
        }

        public override async Task<RoleDto> Update(RoleDto input)
        {
            CheckUpdatePermission();

            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            ObjectMapper.Map(input, role);
            role.SysCateIDs = string.Join(",", input.SysCateIDs);
            CheckErrors(await _roleManager.UpdateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.Permissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
            ClearUserSysCategorysCache();
            return MapToEntityDto(role);
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var role = await _roleManager.FindByIdAsync(input.Id);
            if (role.IsStatic)
            {
                throw new UserFriendlyException("CannotDeleteAStaticRole");
            }

            var users = await GetUsersInRoleAsync(role.Name);

            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.Name));
            }
            ClearUserSysCategorysCache();
            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        private Task<List<long>> GetUsersInRoleAsync(string roleName)
        {
            var users = (from user in _userRepository.GetAll()
                         join userRole in _userRoleRepository.GetAll() on user.Id equals userRole.UserId
                         join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                         where role.Name == roleName
                         select user.Id).Distinct().ToList();

            return Task.FromResult(users);
        }

        public Task<ListResultDto<PermissionDto>> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                ObjectMapper.Map<List<PermissionDto>>(permissions)
            ));
        }


        public List<SysCategory> GetSysCategoryAccordingRoleID()
        {
            var sysCateIDs = (from userRole in _userRoleRepository.GetAll()
                              join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                              where userRole.UserId == AbpSession.UserId
                              select role.SysCateIDs).FirstOrDefault();
            List<SysCategory> sysList = new List<Log2Net.Models.SysCategory>();
            var ids = sysCateIDs.Split(',');
            if (ids.Contains(((int)(SysCategory.ALL)).ToString()))
            {
                sysList = ComClass.GetMySysCategory();
                sysList.Remove(SysCategory.ALL);
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

        protected override IQueryable<Role> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Permissions);
        }

        protected override Task<Role> GetEntityByIdAsync(int id)
        {
            var role = Repository.GetAllIncluding(x => x.Permissions).FirstOrDefault(x => x.Id == id);
            return Task.FromResult(role);
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        void ClearUserSysCategorysCache()
        {
            _cacheManager.GetCache("LgWG.LogQuery").Remove("UserSysCategorys");
        }

    }
}