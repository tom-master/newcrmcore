using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Web.Controllers
{
    public class SecurityController : BaseController
    {
        private readonly ISecurityServices _securityServices;

        private readonly IAppServices _appServices;

        public SecurityController(ISecurityServices securityServices, IAppServices appServices)
        {
            _securityServices = securityServices;
            _appServices = appServices;
        }

        #region 页面

        #region 角色

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateNewRole(Int32 roleId = default(Int32))
        {
            if (roleId != 0)
            {
                ViewData["RoleResult"] = await _securityServices.GetRoleAsync(roleId);
            }
            return View();
        }

        /// <summary>
        /// 向角色附加权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AttachmentPower(Int32 roleId)
        {
            #region 参数验证
            Parameter.Validate(roleId);
            #endregion

            var role = new RoleDto();
            if (roleId != 0)
            {
                role = await _securityServices.GetRoleAsync(roleId);
                ViewData["RolePowerResult"] = role;
            }
            var result = new List<AppDto>();
            if (role.Powers.Any())
            {
                result = await _appServices.GetSystemAppAsync(role.Powers.Select(s => s.Id).ToArray());
            }
            return View(result);
        }

        #endregion

        #region 权限

        /// <summary>
        /// 添加系统应用到权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddSystemAppGotoPower()
        {
            ViewData["SystemApp"] = await _appServices.GetSystemAppAsync();
            return View();
        }

        #endregion

        #endregion

        #region 移除角色

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveRole(Int32 roleId)
        {
            #region 参数验证
            Parameter.Validate(roleId);
            #endregion

            var response = new ResponseModel();
            await _securityServices.RemoveRoleAsync(roleId);
            response.IsSuccess = true;
            response.Message = "移除角色成功";

            return Json(response);
        }

        #endregion

        #region 添加角色

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole(IFormCollection forms, Int32 roleId = 0)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            if (roleId != 0)
            {
                await _securityServices.ModifyRoleAsync(WapperRoleDto(forms));
            }
            else
            {
                await _securityServices.AddNewRoleAsync(WapperRoleDto(forms));
            }
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "添加角色成功"
            };

            return Json(response);
        }

        #endregion

        #region 选择应用

        /// <summary>
        /// 选择应用
        /// </summary>
        /// <param name="appIds"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSystemApp(String appIds)
        {
            #region 参数验证
            Parameter.Validate(appIds);
            #endregion

            var response = new ResponseModel<IList<AppDto>>();
            var internalAppIds = appIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            var result = await _appServices.GetSystemAppAsync(internalAppIds);
            response.IsSuccess = true;
            response.Message = "选择系统app成功";
            response.Model = result;

            return Json(response);
        }

        #endregion

        #region 获取所有角色

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles(String roleName, Int32 pageIndex, Int32 pageSize)
        {
            var response = new ResponseModels<IList<RoleDto>>();
            var result = await _securityServices.GetRolesAsync(roleName, pageIndex, pageSize);
            response.IsSuccess = true;
            response.Message = "获取角色列表成功";
            response.Model = result.Models;
            response.TotalCount = result.TotalCount;

            return Json(response);
        }

        #endregion

        #region 检查角色名称

        /// <summary>
        /// 检查角色名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckName(String param)
        {
            Parameter.Validate(param);

            var result = await _securityServices.CheckRoleNameAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "角色名称已存在" });
        }

        #endregion

        #region 检查角色标识

        /// <summary>
        /// 检查角色标识
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckRoleIdentity(String param)
        {
            Parameter.Validate(param);
            var result = await _securityServices.CheckRoleIdentityAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "角色标识已存在" });
        }

        #endregion

        #region 将应用附加到角色

        /// <summary>
        /// 将应用附加到角色
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAppToRole(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            var response = new ResponseModel();
            var powerIds = forms["val_apps_id"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            if (powerIds.Any())
            {
                await _securityServices.AddPowerToCurrentRoleAsync(Int32.Parse(forms["val_roleId"]), powerIds);
                response.IsSuccess = true;
                response.Message = "将权限附加到角色中成功";
            }
            else
            {
                response.Message = "一个角色至少拥有一个app";
            }

            return Json(response);
        }

        #endregion

        #region private method

        private static RoleDto WapperRoleDto(IFormCollection forms)
        {
            var roleId = 0;
            if ((forms["roleId"] + "").Length > 0)
            {
                roleId = Int32.Parse(forms["roleId"]);
            }

            return new RoleDto
            {
                RoleIdentity = forms["val_roleIdentity"],
                Id = roleId,
                Name = forms["val_roleName"],
                Remark = forms["val_roleRemake"]
            };
        }

        #endregion
    }
}