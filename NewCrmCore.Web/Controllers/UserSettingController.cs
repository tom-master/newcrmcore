using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
    public class UserSettingController : BaseController
    {
        private IUserServices _userServices;

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
        public async Task<IActionResult> Index()
        {
            var user = await _userServices.GetUserAsync(UserId);
            return View(user);
        }

        #endregion

        #region 修改锁屏密码

        /// <summary>
        /// 修改锁屏密码
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyLockPassword(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            await _userServices.ModifyLockScreenPasswordAsync(UserId, forms["lockpassword"]);

            return Json(new ResponseModel
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
        public async Task<IActionResult> ModifyFace(String userFace)
        {
            #region 参数验证
            Parameter.Validate(userFace);
            #endregion

            await _userServices.ModifyUserFaceAsync(UserId, userFace);

            return Json(new ResponseModel
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
        public async Task<IActionResult> ModifyPassword(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            await _userServices.ModifyPasswordAsync(UserId, forms["password"], Int32.Parse(forms["lockPwdIsEqLoginPwd"]) == 1);
            Response.Cookies.Append("User", UserId.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(-1) });

            return Json(new ResponseModel
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
        public async Task<IActionResult> CheckPassword(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckPasswordAsync(UserId, param);
            return Json(result ? new { status = "y", info = "" } : new { status = "n", info = "原始密码错误" });
        }

        #endregion
    }
}