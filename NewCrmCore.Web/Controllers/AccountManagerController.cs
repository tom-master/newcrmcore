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
	public class AccountManagerController : BaseController
	{
		private readonly ISecurityServices _securityServices;
		private readonly IAccountServices _accountServices;

		public AccountManagerController(ISecurityServices securityServices, IAccountServices accountServices)
		{
			_securityServices = securityServices;
			_accountServices = accountServices;
		}

		#region 页面

		/// <summary>
		/// 首页
		/// </summary>
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 创建新账户
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Account(Int32 accountId = 0)
		{
			if (accountId != 0)
			{
				ViewData["Account"] = await _accountServices.GetAccountAsync(accountId);
			}
			ViewData["Roles"] = (await _securityServices.GetRolesAsync("", 1, 100)).Models;
			await CacheHelper.GetOrSetCache(new GlobalUniqueTokenCacheKey(accountId), () => TimeToken.GetTokenAsync());
			return View();
		}

		#endregion

		#region 移除账户

		/// <summary>
		/// 移除账户
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> RemoveAccount(Int32 accountId)
		{
			#region 参数验证
			new Parameter().Validate(accountId);
			#endregion

			var response = new ResponseModel<String>();
			await _accountServices.RemoveAccountAsync(accountId);
			response.IsSuccess = true;
			response.Message = "移除账户成功";

			return Json(response);
		}

		#endregion

		#region 账户启用

		/// <summary>
		/// 账户启用
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Enable(Int32 accountId)
		{
			#region 参数验证
			new Parameter().Validate(accountId);
			#endregion

			var response = new ResponseModel<String>();
			await _accountServices.EnableAsync(accountId);
			response.IsSuccess = true;
			response.Message = "启用账户成功";

			return Json(response);
		}

		#endregion

		#region 账户禁用

		/// <summary>
		/// 账户禁用
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Disable(Int32 accountId)
		{
			#region 参数验证
			new Parameter().Validate(accountId);
			#endregion

			var response = new ResponseModel<String>();
			await _accountServices.DisableAsync(accountId);
			response.IsSuccess = true;
			response.Message = "禁用账户成功";

			return Json(response);
		}

		#endregion

		#region 创建新账户

		/// <summary>
		/// 创建新账户
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateAccount(IFormCollection forms)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var response = new ResponseModel<AccountDto>();
			var dto = WapperAccountDto(forms);
			if (dto.Id == 0)
			{
				await _accountServices.AddNewAccountAsync(dto);

				response.Message = "创建新账户成功";
				response.IsSuccess = true;
			}
			else
			{
				await _accountServices.ModifyAccountAsync(dto);
				if (!String.IsNullOrEmpty(dto.Password))
				{
					Response.Cookies.Append("memberID", AccountId.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
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
		[HttpGet]
		public async Task<IActionResult> Accounts(String accountName, String accountType, Int32 pageIndex, Int32 pageSize)
		{
			var response = new ResponseModels<IList<AccountDto>>();

			var result = await _accountServices.GetAccountsAsync(accountName, accountType, pageIndex, pageSize);
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
		[HttpPost]
		public async Task<IActionResult> CheckName(String param)
		{
			#region 参数验证
			new Parameter().Validate(param);
			#endregion

			var result = await _accountServices.CheckAccountNameExistAsync(param);
			return Json(result ? new { status = "y", info = "" } : new { status = "n", info = "用户名已存在" });
		}

		#endregion

		#region private method

		private AccountDto WapperAccountDto(IFormCollection forms)
		{
			var roleIds = new List<RoleDto>();
			if ((forms["val_roleIds"] + "").Length > 0)
			{
				roleIds = forms["val_roleIds"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(role => new RoleDto
				{
					Id = Int32.Parse(role)
				}).ToList();
			}

			return new AccountDto
			{
				Id = String.IsNullOrEmpty(forms["id"]) ? 0 : Int32.Parse(forms["id"]),
				Name = forms["val_accountname"],
				Password = forms["val_password"],
				IsAdmin = Int32.Parse(forms["val_type"]) == 2,
				Roles = roleIds
			};
		}

		#endregion
	}
}