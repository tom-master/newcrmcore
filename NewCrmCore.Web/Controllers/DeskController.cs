using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewLibCore;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
	public class DeskController: BaseController
	{
		private readonly IDeskServices _deskServices;

		public DeskController(IDeskServices deskServices)
		{
			_deskServices = deskServices;
		}

		#region 页面

		/// <summary>
		/// 页面
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> EditMember(Int32 memberId)
		{
			#region 参数验证
			new Parameter().Validate(memberId);
			#endregion

			var result = await _deskServices.GetMemberAsync(AccountId, memberId);
			return View(result);
		}

		#endregion

		#region 更新图标
		
		/// <summary>
		/// 更新图标
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ModifyIcon(Int32 memberId, String newIcon)
		{
			#region 参数验证
			new Parameter().Validate(memberId).Validate(newIcon);
			#endregion

			var response = new ResponseModel<String>();
			await _deskServices.ModifyMemberIconAsync(AccountId, memberId, newIcon);

			response.IsSuccess = true;
			response.Message = "更新图标成功";
			response.Model = Appsetting.FileUrl + newIcon;

			return Json(response);
		}

		#endregion

		#region 新建文件夹

		/// <summary>
		/// 新建文件夹
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CreateFolder(String folderName, String folderImg, Int32 deskId)
		{
			#region 参数验证
			new Parameter().Validate(folderName).Validate(folderImg).Validate(deskId);
			#endregion

			var response = new ResponseModel();
			await _deskServices.CreateNewFolderAsync(folderName, folderImg, deskId, AccountId);
			response.IsSuccess = true;
			response.Message = "新建文件夹成功";

			return Json(response);
		}

		#endregion

		#region 卸载桌面成员

		/// <summary>
		/// 卸载桌面成员
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> Uninstall(Int32 memberId)
		{
			#region 参数验证
			new Parameter().Validate(memberId);
			#endregion

			var response = new ResponseModel();
			await _deskServices.UninstallMemberAsync(AccountId, memberId);
			response.IsSuccess = true;
			response.Message = "卸载成功";

			return Json(response);
		}

		#endregion

		#region 检查成员名称

		/// <summary>
		/// 检查成员名称
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CheckName(String param)
		{
			#region 参数验证
			new Parameter().Validate(param);
			#endregion

			var result = await _deskServices.CheckMemberNameAsync(param);
			return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "成员名称已存在" });
		}

		#endregion

		#region 桌面成员移动

		/// <summary>
		/// 桌面成员移动
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> MemberMove(String moveType, Int32 memberId, Int32 from, Int32 to)
		{
			#region 参数验证
			new Parameter().Validate(moveType).Validate(memberId);
			#endregion

			switch (moveType)
			{
				case "desk-dock": //成员从桌面移动到码头
					await _deskServices.MemberInDockAsync(AccountId, memberId);
					break;
				case "dock-desk": //成员从码头移动到桌面
					await _deskServices.MemberOutDockAsync(AccountId, memberId, to);
					break;
				case "dock-folder": //成员从码头移动到桌面文件夹中
					await _deskServices.DockToFolderAsync(AccountId, memberId, to);
					break;
				case "folder-dock": //成员从文件夹移动到码头
					await _deskServices.FolderToDockAsync(AccountId, memberId);
					break;
				case "desk-folder": //成员从桌面移动到文件夹
					await _deskServices.DeskToFolderAsync(AccountId, memberId, to);
					break;
				case "folder-desk": //成员从文件夹移动到桌面
					await _deskServices.FolderToDeskAsync(AccountId, memberId, to);
					break;
				case "folder-folder": //成员从文件夹移动到另一个文件夹中
					await _deskServices.FolderToOtherFolderAsync(AccountId, memberId, to);
					break;
				case "desk-desk": //桌面移动到另一个桌面
					await _deskServices.DeskToOtherDeskAsync(AccountId, memberId, to);
					break;
				case "dock-otherdesk"://应用码头移动到另一个桌面
					await _deskServices.DockToOtherDeskAsync(AccountId, memberId, to);
					break;
			}
			var response = new ResponseModel
			{
				IsSuccess = true,
				Message = "移动成功"
			};

			return Json(response);
		}

		#endregion

		#region 修改成员信息

		/// <summary>
		/// 修改成员信息
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ModifyMemberInfo(IFormCollection forms)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var memberDto = new MemberDto
			{
				Id = Int32.Parse(forms["id"]),
				IconUrl = forms["val_icon"],
				Name = forms["val_name"],
				Width = Int32.Parse(forms["val_width"]),
				Height = Int32.Parse(forms["val_height"]),
				IsResize = Int32.Parse(forms["val_isresize"]) == 1,
				IsOpenMax = Int32.Parse(forms["val_isopenmax"]) == 1,
				IsFlash = Int32.Parse(forms["val_isflash"]) == 1,
				MemberType = forms["membertype"],
				IsIconByUpload = Int32.Parse(forms["isIconByUpload"]) == 1
			};

			var response = new ResponseModel();
			await _deskServices.ModifyMemberInfoAsync(AccountId, memberDto);
			response.IsSuccess = true;
			response.Message = "修改成员信息成功";

			return Json(response);
		}

		#endregion
		 
		#region 修改文件夹信息

		/// <summary>
		/// 修改文件夹信息
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> ModifyFolderInfo(String name, String icon, Int32 memberId)
		{
			#region 参数验证
			new Parameter().Validate(name).Validate(icon).Validate(memberId);
			#endregion

			var response = new ResponseModel();
			await _deskServices.ModifyFolderInfoAsync(AccountId, name, icon, memberId);
			response.IsSuccess = true;
			response.Message = "修改成功";

			return Json(response);
		}

		#endregion

	}
}