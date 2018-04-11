using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Web.Controllers.ControllerHelper;

namespace NewCrmCore.Web.Controllers
{
	public class AccountManagerController: BaseController
	{
		private readonly ISecurityServices _securityServices;

		public AccountManagerController(ISecurityServices securityServices)
		{
			_securityServices = securityServices;
		}

		#region 页面

		/// <summary>
		/// 首页
		/// </summary>
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 创建新账户
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> CreateNewAccount(Int32 accountId = 0)
		{
			if (accountId != 0)
			{
				ViewData["Account"] = await AccountServices.GetAccountAsync(accountId);
			}

			ViewData["Roles"] = _securityServices.GetRoles("", 1, 100, out var totalCount);
			return View();
		}

		#endregion

		/// <summary>
		/// 获取所有账户
		/// </summary>
		[HttpGet]
		public ActionResult Accounts(String accountName, String accountType, Int32 pageIndex, Int32 pageSize)
		{
			var response = new ResponseModels<IList<AccountDto>>();

			#region 参数验证
			new Parameter().Validate(accountName).Validate(accountType);
			#endregion

			var accounts = AccountServices.GetAccounts(accountName, accountType, pageIndex, pageSize, out var totalCount);
			if (accounts != null)
			{
				response.TotalCount = totalCount;
				response.Message = "获取账户列表成功";
				response.Model = accounts;
				response.IsSuccess = true;
			}

			return Json(response, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 创建新账户
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> NewAccount(FormCollection forms)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var response = new ResponseModel<AccountDto>();
			var dto = WapperAccountDto(forms);
			if (dto.Id == 0)
			{
				await AccountServices.AddNewAccountAsync(dto);

				response.Message = "创建新账户成功";
				response.IsSuccess = true;
			}
			else
			{
				await AccountServices.ModifyAccountAsync(dto);
				if (!String.IsNullOrEmpty(dto.Password))
				{
					Response.Cookies.Add(new HttpCookie("memberID")
					{
						Value = AccountId.ToString(),
						Expires = DateTime.Now.AddDays(-1)
					});
				}
				response.Message = "修改账户成功";
				response.IsSuccess = true;
			}


			return Json(response);
		}

		/// <summary>
		/// 检查账户名是否已经存在
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CheckAccountNameExist(String param)
		{
			#region 参数验证
			new Parameter().Validate(param);
			#endregion

			var result = await AccountServices.CheckAccountNameExistAsync(param);
			return Json(result ? new { status = "y", info = "" } : new { status = "n", info = "用户名已存在" });
		}

		/// <summary>
		/// 移除账户
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> RemoveAccount(Int32 accountId)
		{
			#region 参数验证
			new Parameter().Validate(accountId);
			#endregion

			var response = new ResponseModel<String>();
			await AccountServices.RemoveAccountAsync(accountId);
			response.IsSuccess = true;
			response.Message = "移除账户成功";

			return Json(response, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 修改账户为禁用状态
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ChangeAccountStatus(Int32 accountId, String isDisable)
		{
			#region 参数验证
			new Parameter().Validate(accountId).Validate(isDisable);
			#endregion

			var response = new ResponseModel<String>();
			if (!Boolean.Parse(isDisable))
			{
				await AccountServices.DisableAsync(accountId);
				response.IsSuccess = true;
				response.Message = "禁用账户成功";
			}
			else
			{
				await AccountServices.EnableAsync(accountId);
				response.IsSuccess = true;
				response.Message = "启用账户成功";
			}

			return Json(response, JsonRequestBehavior.AllowGet);
		}

		#region private method

		private AccountDto WapperAccountDto(FormCollection forms)
		{
			var roleIds = new List<RoleDto>();
			if ((forms["val_roleIds"] + "").Length > 0)
			{
				roleIds = forms["val_roleIds"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(role => new RoleDto
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