using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Web.Controllers.ControllerHelper;

namespace NewCrmCore.Web.Controllers
{
	public class SecurityController: BaseController
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
		/// 首页
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 创建新角色
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> CreateNewRole(Int32 roleId = default(Int32))
		{
			if (roleId != 0)
			{
				ViewData["RoleResult"] = await _securityServices.GetRoleAsync(roleId);
			}

			return View();
		}

		/// <summary>
		/// 向角色附加权限
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> AttachmentPowerAsync(Int32 roleId)
		{
			#region 参数验证
			new Parameter().Validate(roleId);
			#endregion

			var role = new RoleDto();
			if (roleId != 0)
			{
				role = await _securityServices.GetRoleAsync(roleId);
				ViewData["RolePowerResult"] = role;
			}

			var result = await _appServices.GetSystemAppAsync(role.Powers.Select(s => s.Id).ToArray());
			return View(result);
		}

		#endregion

		#region 权限

		/// <summary>
		/// 添加系统app到权限
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> AddSystemAppGotoPower()
		{
			ViewData["SystemApp"] = await _appServices.GetSystemAppAsync();
			return View();
		}

		#endregion

		#endregion

		/// <summary>
		/// 获取所有的角色
		/// </summary>
		[HttpGet]
		public ActionResult GetRoles(String roleName, Int32 pageIndex, Int32 pageSize)
		{
			#region 参数验证
			new Parameter().Validate(roleName);
			#endregion

			var response = new ResponseModels<IList<RoleDto>>();
			var result = _securityServices.GetRoles(roleName, pageIndex, pageSize, out var totalCount);
			response.IsSuccess = true;
			response.Message = "获取角色列表成功";
			response.Model = result;
			response.TotalCount = totalCount;

			return Json(response, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 移除角色
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> RemoveRole(Int32 roleId)
		{
			#region 参数验证
			new Parameter().Validate(roleId);
			#endregion

			var response = new ResponseModel();
			await _securityServices.RemoveRoleAsync(roleId);
			response.IsSuccess = true;
			response.Message = "移除角色成功";

			return Json(response);
		}

		/// <summary>
		/// 添加角色
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CreateRole(FormCollection forms, Int32 roleId = 0)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			if (roleId != 0)
			{
				await _securityServices.ModifyRoleAsync(WapperRoleDto(forms));
			}
			else
			{
				await _securityServices.AddNewRoleAsync(WapperRoleDto(forms));
			}
			var response = new ResponseModel
			{
				IsSuccess = true,
				Message = "添加角色成功"
			};

			return Json(response);
		}

		/// <summary>
		/// 将权限附加到角色中
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> AddPowerToRole(FormCollection forms)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var response = new ResponseModel();
			var powerIds = forms["val_apps_id"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
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

		/// <summary>
		/// 选择系统app
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetSystemApp(String appIds)
		{
			var response = new ResponseModel<IList<AppDto>>();
			var internalAppIds = appIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
			var result = await _appServices.GetSystemAppAsync(internalAppIds);
			response.IsSuccess = true;
			response.Message = "选择系统app成功";
			response.Model = result;

			return Json(response, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 检查角色名称
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CheckRoleName(String param)
		{
			new Parameter().Validate(param);
			var result = await _securityServices.CheckRoleNameAsync(param);
			return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "角色名称已存在" });
		}

		/// <summary>
		/// 检查角色标识
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CheckRoleIdentity(String param)
		{
			new Parameter().Validate(param);
			var result = await _securityServices.CheckRoleIdentityAsync(param);
			return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "角色标识已存在" });
		}


		#region private method

		private static RoleDto WapperRoleDto(FormCollection forms)
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