using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.WebApi.ApiHelper;
using NewLibCore.Validate;

namespace NewCrmCore.WebApi.Controllers
{
    public class UserManagerController : NewCrmController
    {
        private readonly ISecurityServices _securityServices;

        private readonly IUserServices _userServices;

        public UserManagerController(ISecurityServices securityServices, IUserServices userServices)
        {
            _securityServices = securityServices;
            _userServices = userServices;
        }

        #region 页面

        /// <summary>
        /// 创建新账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitUserAsync(Int32 userId = 0)
        {
            UserDto userDto = null;
            var response = new ResponseModel<dynamic>();
            if (userId != 0)
            {
                userDto = await _userServices.GetUserAsync(userId);
                if (userDto == null)
                {
                    response.Message = "获取用户失败";
                    response.IsSuccess = false;
                }
            }
            var userContext = await GetUserContextAsync();
            var roleDtos = await _securityServices.GetRolesAsync("", 1, 100);
            var uniqueToken = await CreateUniqueTokenAsync(userContext.Id);

            response.Model = new { roleDtos, userDto, uniqueToken };
            response.IsSuccess = true;
            response.Message = "创建新账户初始化成功";

            return Json(response);
        }

        #endregion

        #region 移除账户

        /// <summary>
        /// 移除账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveUserAsync(Int32 userId)
        {
            #region 参数验证
            Check.IfNullOrZero(userId);
            #endregion

            await _userServices.RemoveUserAsync(userId);
            return Json(new ResponseModel<String>
            {
                IsSuccess = true,
                Message = "移除账户成功"
            });
        }

        #endregion

        #region 账户启用

        /// <summary>
        /// 账户启用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EnableAsync(Int32 userId)
        {
            #region 参数验证
            Check.IfNullOrZero(userId);
            #endregion

            await _userServices.EnableAsync(userId);
            return Json(new ResponseModel<String>
            {
                IsSuccess = true,
                Message = "启用账户成功"
            });
        }

        #endregion

        #region 账户禁用

        /// <summary>
        /// 账户禁用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DisableAsync(Int32 userId)
        {
            #region 参数验证
            Check.IfNullOrZero(userId);
            #endregion

            await _userServices.DisableAsync(userId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "禁用账户成功"
            });
        }

        #endregion

        #region 创建新账户

        /// <summary>
        /// 创建新账户
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(IFormCollection forms)
        {
            #region 参数验证
            Check.IfNullOrZero(forms);
            #endregion

            var response = new ResponseModel<UserDto>();
            var dto = WapperUserDto(forms);
            if (dto.Id == 0)
            {
                await _userServices.AddNewUserAsync(dto);

                response.Message = "创建新账户成功";
                response.IsSuccess = true;
            }
            else
            {
                await _userServices.ModifyUserAsync(dto);
                if (!String.IsNullOrEmpty(dto.Password))
                {
                    InternalLogout();
                }
                response.Message = "修改账户成功";
                response.IsSuccess = true;
            }


            return Json(response);
        }

        #endregion

        #region 获取所有账户

        /// <summary>
        /// 获取所有账户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UsersAsync(String userName, String userType, Int32 pageIndex, Int32 pageSize)
        {
            Check.IfNullOrZero(userName, true);
            Check.IfNullOrZero(userType, true);
            var response = new ResponsePaging<IList<UserDto>>();
            var result = await _userServices.GetUsersAsync(userName, userType, pageIndex, pageSize);
            if (result != null)
            {
                response.TotalCount = result.TotalCount;
                response.Message = "获取账户列表成功";
                response.Model = result.Models;
                response.IsSuccess = true;
            }

            return Json(response);
        }

        #endregion

        #region 检查账户名是否已经存在

        /// <summary>
        /// 检查账户名是否已经存在
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckNameAsync(String param)
        {
            #region 参数验证
            Check.IfNullOrZero(param);
            #endregion

            var result = await _userServices.CheckUserNameExistAsync(param);
            var response = new ResponseSimple();
            if (!result)
            {
                response.Model = "y";
                response.IsSuccess = true;
                response.Message = "";
            }
            else
            {
                response.Model = "n";
                response.IsSuccess = false;
                response.Message = "用户名已存在";
            }
            return Json(response);
        }

        #endregion

        #region private method

        private UserDto WapperUserDto(IFormCollection forms)
        {
            var roleIds = new List<RoleDto>();
            if ((forms["val_roleIds"] + "").Length > 0)
            {
                roleIds = forms["val_roleIds"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(role => new RoleDto
                {
                    Id = Int32.Parse(role)
                }).ToList();
            }

            return new UserDto
            {
                Id = String.IsNullOrEmpty(forms["id"]) ? 0 : Int32.Parse(forms["id"]),
                Name = forms["val_username"],
                Password = forms["val_password"],
                IsAdmin = Int32.Parse(forms["val_type"]) == 2,
                Roles = roleIds
            };
        }

        #endregion
    }
}