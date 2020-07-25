using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.WebApi.ApiHelper;
using NewLibCore.Validate;

namespace NewCrmCore.WebApi.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class UserSettingController : NewCrmController
    {
        private readonly IUserServices _userServices;

        public UserSettingController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        #region 页面

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitUserSettingAsync()
        {
            var user = await _userServices.GetUserAsync(UserInfo.Id);
            var uniqueToken = await CreateUniqueTokenAsync(UserInfo.Id);
            return Json(new ResponseModel<dynamic>
            {
                Model = new { user, uniqueToken },
                Message = "用户设置初始化成功",
                IsSuccess = true
            });
        }

        #endregion

        #region 修改锁屏密码

        /// <summary>
        /// 修改锁屏密码
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyLockPasswordAsync(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            await _userServices.ModifyLockScreenPasswordAsync(UserInfo.Id, forms["lockpassword"]);

            return Json(new ResponseSimple
            {
                Message = "锁屏密码修改成功",
                IsSuccess = true,
            });
        }

        #endregion

        #region 上传账户头像

        /// <summary>
        /// 上传账户头像
        /// </summary>
        /// <param name="userFace"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyFaceAsync(String userFace)
        {
            #region 参数验证
            Parameter.Validate(userFace);
            #endregion

            await _userServices.ModifyUserFaceAsync(UserInfo.Id, userFace);

            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Model = "头像上传成功"
            });
        }

        #endregion

        #region 修改账户登陆密码

        /// <summary>
        /// 修改账户登陆密码
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyPasswordAsync(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            await _userServices.ModifyPasswordAsync(UserInfo.Id, forms["password"], Int32.Parse(forms["lockPwdIsEqLoginPwd"]) == 1);
            InternalLogout();
            return Json(new ResponseSimple
            {
                Message = "账户密码修改成功",
                IsSuccess = true
            });
        }

        #endregion

        #region 检查旧密码和输入的密码是否一致

        /// <summary>
        /// 检查旧密码和输入的密码是否一致
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckPasswordAsync(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckPasswordAsync(UserInfo.Id, param);
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
                response.Message = "原始密码错误";
            }
            return Json(response);
        }

        #endregion
    }
}