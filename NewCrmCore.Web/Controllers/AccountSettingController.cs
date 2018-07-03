using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
	public class AccountSettingController : BaseController
	{
		private IAccountServices _accountServices;

		public AccountSettingController(IAccountServices accountServices)
		{
			_accountServices = accountServices;
		}

		#region 页面

		/// <summary>
		/// 首页
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var account = await _accountServices.GetAccountAsync(AccountId);
			return View(account);
		}

		#endregion

		#region 修改锁屏密码

		/// <summary>
		/// 修改锁屏密码
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyLockPassword(IFormCollection forms)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var response = new ResponseModel();
			await _accountServices.ModifyLockScreenPasswordAsync(AccountId, forms["lockpassword"]);

			response.Message = "锁屏密码修改成功";
			response.IsSuccess = true;

			return Json(response);
		}

		#endregion

		#region 上传账户头像

		/// <summary>
		///上传账户头像
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyFace(String accountFace)
		{
			#region 参数验证
			new Parameter().Validate(accountFace);
			#endregion

			var response = new ResponseModel();
			await _accountServices.ModifyAccountFaceAsync(AccountId, accountFace);
			response.IsSuccess = true;
			response.Model = "头像上传成功";

			return Json(response);
		}

		#endregion

		#region 修改账户登陆密码

		/// <summary>
		/// 修改账户登陆密码
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyPassword(IFormCollection forms)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var response = new ResponseModel();

			await _accountServices.ModifyPasswordAsync(AccountId, forms["password"], Int32.Parse(forms["lockPwdIsEqLoginPwd"]) == 1);
			InternalLogout();

			response.Message = "账户密码修改成功";
			response.IsSuccess = true;

			return Json(response);
		}

		#endregion

		#region 检查旧密码和输入的密码是否一致

		/// <summary>
		/// 检查旧密码和输入的密码是否一致
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CheckPassword(String param)
		{
			#region 参数验证
			new Parameter().Validate(param);
			#endregion

			var result = await _accountServices.CheckPasswordAsync(AccountId, param);
			return Json(result ? new { status = "y", info = "" } : new { status = "n", info = "原始密码错误" });
		}

		#endregion
	}
}