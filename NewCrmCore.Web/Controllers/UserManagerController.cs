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
    public class UserManagerController : BaseController
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
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 创建新账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public new async Task<IActionResult> User(Int32 userId = 0)
        {
            if (userId != 0)
            {
                ViewData["User"] = await _userServices.GetUserAsync(userId);
            }
            ViewData["Roles"] = (await _securityServices.GetRolesAsync("", 1, 100)).Models;
            return View();
        }

        #endregion

        #region 移除账户

        /// <summary>
        /// 移除账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveUser(Int32 userId)
        {
            #region 参数验证
            Parameter.Validate(userId);
            #endregion

            var response = new ResponseModel<String>();
            await _userServices.RemoveUserAsync(userId);
            response.IsSuccess = true;
            response.Message = "移除账户成功";

            return Json(response);
        }

        #endregion

        #region 账户启用

        /// <summary>
        /// 账户启用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(Int32 userId)
        {
            #region 参数验证
            Parameter.Validate(userId);
            #endregion

            var response = new ResponseModel<String>();
            await _userServices.EnableAsync(userId);
            response.IsSuccess = true;
            response.Message = "启用账户成功";

            return Json(response);
        }

        #endregion

        #region 账户禁用

        /// <summary>
        /// 账户禁用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Disable(Int32 userId)
        {
            #region 参数验证
            Parameter.Validate(userId);
            #endregion

            var response = new ResponseModel<String>();
            await _userServices.DisableAsync(userId);
            response.IsSuccess = true;
            response.Message = "禁用账户成功";

            return Json(response);
        }

        #endregion

        #region 创建新账户

        /// <summary>
        /// 创建新账户
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
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
                    Response.Cookies.Append("User", UserId.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
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
        public async Task<IActionResult> Users(String userName, String userType, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.Validate(userName, true);
            Parameter.Validate(userType, true);
            var response = new ResponseModels<IList<UserDto>>();
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
        public async Task<IActionResult> CheckName(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckUserNameExistAsync(param);
            return Json(result ? new { status = "y", info = "" } : new { status = "n", info = "用户名已存在" });
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