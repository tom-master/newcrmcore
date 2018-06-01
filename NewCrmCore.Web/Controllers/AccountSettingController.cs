﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
	public class AccountSettingController: BaseController
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
		public async Task<ActionResult> Index()
		{
			var account = await _accountServices.GetAccountAsync(AccountId);
			return View(account);
		}

		#endregion

		/// <summary>
		///上传账户头像
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ModifyAccountFace(String accountFace)
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

		/// <summary>
		/// 修改账户登陆密码
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ModifyAccountPassword(IFormCollection forms)
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

		/// <summary>
		/// 修改锁屏密码
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ModifyLockScreenPassword(IFormCollection forms)
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

		/// <summary>
		/// 检查旧密码和输入的密码是否一致
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CheckPassword(String param)
		{
			#region 参数验证
			new Parameter().Validate(param);
			#endregion

			var result = await _accountServices.CheckPasswordAsync(AccountId, param);
			return Json(result ? new { status = "y", info = "" } : new { status = "n", info = "原始密码错误" });
		}
	}
}