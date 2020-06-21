using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
    public class SecurityController : NewCrmController
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
        /// 创建新角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitCreateNewRoleAsync(Int32 roleId = 0)
        {
            var response = new ResponseModel<dynamic>();
            RoleDto roleDto = null;
            if (roleId != 0)
            {
                roleDto = await _securityServices.GetRoleAsync(roleId);
                if (roleDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "角色初始化失败";
                    return Json(response);
                }
            }
            var uniqueToken = CreateUniqueTokenAsync(UserInfo.Id);
            response.Model = new { roleDto, uniqueToken };
            response.IsSuccess = true;
            response.Message = "创建新角色初始化成功";
            return Json(response);
        }

        /// <summary>
        /// 向角色附加权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitAttachmentPowerAsync(Int32 roleId)
        {
            #region 参数验证
            Parameter.Validate(roleId);
            #endregion

            var roleDto = new RoleDto();
            var response = new ResponseModel<dynamic>();
            if (roleId != 0)
            {
                roleDto = await _securityServices.GetRoleAsync(roleId);
                if (roleDto == null)
                {
                    response.IsSuccess = true;
                    response.Message = "获取角色失败";
                    return Json(response);
                }
            }
            var appDtos = new List<AppDto>();
            if (roleDto.Powers.Any())
            {
                appDtos = await _appServices.GetSystemAppAsync(roleDto.Powers.Select(s => s.Id).ToArray());
                if (appDtos == null)
                {
                    response.IsSuccess = false;
                    response.Message = "获取角色对应的系统应用失败";
                    return Json(response);
                }
            }
            var uniqueToken = CreateUniqueTokenAsync(UserInfo.Id);

            response.Model = new { roleDto, appDtos, uniqueToken };
            response.IsSuccess = true;
            response.Message = "附加角色权限初始化成功";

            return Json(response);
        }

        #endregion

        #region 权限

        /// <summary>
        /// 添加系统应用到权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddSystemAppGotoPowerAsync()
        {
            var systemApps = await _appServices.GetSystemAppAsync();
            var response = new ResponseModel<dynamic>();
            if (systemApps == null || !systemApps.Any())
            {
                response.Message = "获取系统应用失败";
                response.IsSuccess = true;
                return Json(response);
            }

            response.Model = new { systemApps };
            response.IsSuccess = true;
            response.Message = "添加系统应用到权限初始化成功";
            return Json(response);
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
        public async Task<IActionResult> RemoveRoleAsync(Int32 roleId)
        {
            #region 参数验证
            Parameter.Validate(roleId);
            #endregion

            await _securityServices.RemoveRoleAsync(roleId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "移除角色成功"
            });
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
        public async Task<IActionResult> CreateRoleAsync(IFormCollection forms, Int32 roleId = 0)
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
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "添加角色成功"
            });
        }

        #endregion

        #region 选择应用

        /// <summary>
        /// 选择应用
        /// </summary>
        /// <param name="appIds"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSystemAppAsync(String appIds)
        {
            #region 参数验证
            Parameter.Validate(appIds);
            #endregion

            var internalAppIds = appIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            var result = await _appServices.GetSystemAppAsync(internalAppIds);

            return Json(new ResponseModel<IList<AppDto>>
            {
                IsSuccess = true,
                Message = "选择系统app成功",
                Model = result
            });
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
        public async Task<IActionResult> GetRolesAsync(String roleName, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.Validate(roleName, true);
            var result = await _securityServices.GetRolesAsync(roleName, pageIndex, pageSize);
            return Json(new ResponsePaging<IList<RoleDto>>
            {
                IsSuccess = true,
                Message = "获取角色列表成功",
                Model = result.Models,
                TotalCount = result.TotalCount
            });
        }

        #endregion

        #region 检查角色名称

        /// <summary>
        /// 检查角色名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckNameAsync(String param)
        {
            Parameter.Validate(param);
            var result = await _securityServices.CheckRoleNameAsync(param);
            var response = new ResponseSimple();
            if (!result)
            {
                response.Model = "y";
                response.IsSuccess = true;
                response.Model = "";
            }
            else
            {
                response.Model = "n";
                response.IsSuccess = false;
                response.Model = "角色名称已存在";
            }
            return Json(response);
        }

        #endregion

        #region 检查角色标识

        /// <summary>
        /// 检查角色标识
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckRoleIdentityAsync(String param)
        {
            Parameter.Validate(param);
            var result = await _securityServices.CheckRoleIdentityAsync(param);
            var response = new ResponseSimple();
            if (!result)
            {
                response.Model = "y";
                response.IsSuccess = true;
                response.Model = "";
            }
            else
            {
                response.Model = "n";
                response.IsSuccess = false;
                response.Model = "角色标识已存在";
            }
            return Json(response);
        }

        #endregion

        #region 将应用附加到角色

        /// <summary>
        /// 将应用附加到角色
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAppToRoleAsync(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            var response = new ResponseSimple();
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