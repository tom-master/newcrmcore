using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewCrmCore.Web.Filter;
using NewLibCore.Validate;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Controllers
{
	public class DesktopController: BaseController
	{
		private readonly IDeskServices _deskServices;
		private readonly IAccountServices _accountServices;

		public DesktopController(IDeskServices deskServices, IAccountServices accountServices)
		{
			_deskServices = deskServices;
			_accountServices = accountServices;
		}

		#region 页面

		/// <summary>
		/// 首页
		/// </summary>
		/// <returns></returns>
		[HttpGet, DoNotCheckPermission]
		public async Task<ActionResult> Index()
		{
			ViewBag.Title = "桌面";
			if (HttpContext.Request.Cookies["memberID"] != null)
			{
				var account = await _accountServices.GetAccountAsync(AccountId);
				account.AccountFace = Appsetting.FileUrl + account.AccountFace;
				ViewData["Account"] = account;
				ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(account.Id);
				ViewData["Desks"] = (await _accountServices.GetConfigAsync(account.Id)).DefaultDeskCount;

				return View(account);
			}

			return RedirectToAction("Login", "Desktop");
		}

		/// <summary>
		/// 首页
		/// </summary>
		/// <returns></returns>
		[HttpGet, DoNotCheckPermission]
		public ActionResult Login()
		{
			var accountId = Request.Cookies["memberID"];
			if (accountId != null)
			{
				return RedirectToAction("Index", "Desktop");
			}

			return View();
		}


		#endregion

		#region 登陆

		/// <summary>
		/// 登陆
		/// </summary>
		[HttpPost, DoNotCheckPermission]
		public async Task<ActionResult> Landing(LoginParameter loginParameter)
		{
			#region 参数验证
			new Parameter().Validate(loginParameter);
			#endregion

			var response = new ResponseModel<AccountDto>();

			var account = await _accountServices.LoginAsync(loginParameter.Name, loginParameter.Password, Request.HttpContext.Connection.RemoteIpAddress.ToString());
			if (account != null)
			{
				var cookieTimeout = loginParameter.IsRememberPasswrod ? DateTime.Now.AddDays(7) : DateTime.Now.AddMinutes(30);
				response.Message = "登陆成功";
				response.IsSuccess = true;

				HttpContext.Response.Cookies.Append("memberID", account.Id.ToString(), new CookieOptions { Expires = cookieTimeout });
				HttpContext.Response.Cookies.Append("Account", JsonConvert.SerializeObject(new { AccountFace = Appsetting.FileUrl + account.AccountFace, account.Name }), new CookieOptions { Expires = cookieTimeout });
			}
			return Json(response);
		}

		#endregion

		#region 解锁屏幕

		/// <summary>
		/// 解锁屏幕
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> UnlockScreen(String unlockPassword)
		{
			#region 参数验证
			new Parameter().Validate(unlockPassword);
			#endregion

			var response = new ResponseModel();
			var result = await _accountServices.UnlockScreenAsync(AccountId, unlockPassword);
			if (result)
			{
				response.IsSuccess = true;
			}

			return Json(response);
		}

		#endregion

		#region 账户登出

		/// <summary>
		/// 账户登出
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> Logout()
		{
			await _accountServices.LogoutAsync(AccountId);
			InternalLogout();
			return new EmptyResult();
		}

		#endregion

		#region 获取皮肤

		/// <summary>
		/// 获取皮肤
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetSkin()
		{
			var response = new ResponseModel<String>();
			var skinName = (await _accountServices.GetConfigAsync(AccountId)).Skin;
			response.IsSuccess = true;
			response.Model = skinName;
			response.Message = "初始化皮肤成功";

			return Json(response);
		}

		#endregion

		#region 获取壁纸

		/// <summary>
		/// 获取壁纸
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetWallpaper()
		{
			var response = new ResponseModel<ConfigDto>();
			var result = await _accountServices.GetConfigAsync(AccountId);

			if (result.IsBing)
			{
				result.WallpaperSource = WallpaperSource.Bing.ToString().ToLower();
				result.WallpaperUrl = await BingHelper.GetEverydayBackgroundImageAsync();
			}

			response.IsSuccess = true;
			response.Message = "初始化壁纸成功";
			response.Model = result;

			return Json(response);
		}

		#endregion

		#region 创建窗口

		/// <summary>
		/// 创建窗口
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> CreateWindow(Int32 id, String type)
		{

			#region 参数验证
			new Parameter().Validate(id).Validate(type);
			#endregion

			var response = new ResponseModel<dynamic>();
			var internalMemberResult = type == "folder" ? await _deskServices.GetMemberAsync(AccountId, id, true) : await _deskServices.GetMemberAsync(AccountId, id);
			response.IsSuccess = true;
			response.Message = "创建一个窗口成功";
			response.Model = new
			{
				type = internalMemberResult.MemberType.ToLower(),
				memberId = internalMemberResult.Id,
				appId = internalMemberResult.AppId,
				name = internalMemberResult.Name,
				icon = internalMemberResult.IsIconByUpload ? Appsetting.FileUrl + internalMemberResult.IconUrl : internalMemberResult.IconUrl,
				width = internalMemberResult.Width,
				height = internalMemberResult.Height,
				isOnDock = internalMemberResult.IsOnDock,
				isDraw = internalMemberResult.IsDraw,
				isOpenMax = internalMemberResult.IsOpenMax,
				isSetbar = internalMemberResult.IsSetbar,
				url = internalMemberResult.AppUrl,
				isResize = internalMemberResult.IsResize
			};

			return Json(response);
		}

		#endregion

		#region 获取账户头像

		/// <summary>
		/// 获取账户头像
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetAccountFace()
		{
			var response = new ResponseModel<String>();
			var result = (await _accountServices.GetConfigAsync(AccountId)).AccountFace;
			response.IsSuccess = true;
			response.Message = "获取用户头像成功";
			response.Model = Appsetting.FileUrl + result;

			return Json(response);
		}

		#endregion

		#region 初始化应用码头

		/// <summary>
		/// 初始化应用码头
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetDockPos()
		{
			var response = new ResponseModel<String>();
			var result = (await _accountServices.GetConfigAsync(AccountId)).DockPosition;
			response.IsSuccess = true;
			response.Message = "初始化应用码头成功";
			response.Model = result;

			return Json(response);
		}

		#endregion

		#region 获取账户安装的应用

		/// <summary>
		/// 获取账户安装的应用
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetAccountDeskMembers()
		{
			var response = new ResponseModel<IDictionary<String, IList<dynamic>>>();
			var result = await _deskServices.GetDeskMembersAsync(AccountId);
			response.IsSuccess = true;
			response.Message = "获取我的应用成功";
			response.Model = result;

			return Json(response);
		}

		#endregion

	}
}