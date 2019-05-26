using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using LgWG.LogQuery.Authorization;
using LgWG.LogQuery.Roles;
using LgWG.LogQuery.Web.Models.Roles;
using Log2Net;
using Log2Net.Models;

namespace LgWG.LogQuery.Web.Controllers
{

    [AbpMvcAuthorize(PermissionNames.Pages_Roles)]
    public class RolesController : LogQueryControllerBase
    {
        private readonly IRoleAppService _roleAppService;

        public RolesController(IRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }


        public async Task<ActionResult> Index()
        {
            var roles = (await _roleAppService.GetAll(new PagedAndSortedResultRequestDto())).Items;
            var permissions = (await _roleAppService.GetAllPermissions()).Items;

            List<SysCategory> cateList = _roleAppService.GetSysCategoryAccordingRoleID();
            cateList.Remove(SysCategory.ALL);
            var model = new RoleListViewModel
            {
                Roles = roles,
                Permissions = permissions,
                SysCateIDs = cateList,
            };
            return View(model);
        }




        public async Task<ActionResult> EditRoleModal(int roleId)
        {
            var role = await _roleAppService.Get(new EntityDto(roleId));
            var permissions = (await _roleAppService.GetAllPermissions()).Items;

            List<SysCategory> cateList = _roleAppService.GetSysCategoryAccordingRoleID();
            cateList.Remove(SysCategory.ALL);
            var model = new EditRoleModalViewModel
            {
                Role = role,
                Permissions = permissions,
                SysCateIDs = cateList,
            };
            return View("_EditRoleModal", model);
        }
    }
}