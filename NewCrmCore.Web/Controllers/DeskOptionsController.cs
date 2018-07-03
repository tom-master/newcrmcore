using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
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
	public class DeskOptionsController : BaseController
	{

		private readonly IWallpaperServices _wallpaperServices;
		private readonly ISkinServices _skinServices;
		private readonly IDeskServices _deskServices;
		private readonly IAppServices _appServices;
		private readonly IAccountServices _accountServices;

		public DeskOptionsController(IWallpaperServices wallpaperServices,
			ISkinServices skinServices,
			IDeskServices deskServices,
			IAppServices appServices,
			IAccountServices accountServices)
		{
			_wallpaperServices = wallpaperServices;
			_skinServices = skinServices;
			_deskServices = deskServices;
			_appServices = appServices;
			_accountServices = accountServices;
		}


		#region 页面

		/// <summary>
		/// 首页
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SystemWallPaper()
		{
			ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(AccountId);
			ViewData["Wallpapers"] = await _wallpaperServices.GetWallpapersAsync();

			return View();
		}

		/// <summary>
		/// 自定义壁纸
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> CustomWallPaper()
		{
			ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(AccountId);
			return View();
		}

		/// <summary>
		/// 设置皮肤
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult SetSkin()
		{
			return View();
		}

		/// <summary>
		/// 程序设置
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> DeskSet()
		{
			ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(AccountId);
			ViewData["Desks"] = (await _accountServices.GetConfigAsync(AccountId)).DefaultDeskCount;

			return View();
		}
		#endregion

		#region 设置壁纸

		/// <summary>
		/// 设置壁纸
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyWallpaper(Int32 wallpaperId)
		{
			#region 参数验证
			new Parameter().Validate(wallpaperId);
			#endregion

			var response = new ResponseModel();
			await _wallpaperServices.ModifyWallpaperAsync(AccountId, wallpaperId);
			response.IsSuccess = true;
			response.Message = "设置壁纸成功";

			return Json(response);
		}

		#endregion

		#region 删除壁纸

		/// <summary>
		/// 删除壁纸
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> RemoveWallpaper(Int32 wallPaperId)
		{
			#region 参数验证
			new Parameter().Validate(wallPaperId);
			#endregion

			var response = new ResponseModel<IList<WallpaperDto>>();
			await _wallpaperServices.RemoveWallpaperAsync(AccountId, wallPaperId);
			response.IsSuccess = true;
			response.Message = "删除壁纸成功";

			return Json(response);
		}

		#endregion

		#region 上传壁纸

		/// <summary>
		/// 上传壁纸     
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> UploadWallPaper(WallpaperDto wallpaper)
		{
			#region 参数验证
			new Parameter().Validate(wallpaper);
			#endregion

			var response = new ResponseModel<dynamic>();

			var wallpaperResult = await _wallpaperServices.AddWallpaperAsync(new WallpaperDto
			{
				Title = wallpaper.Title.Substring(0, 9),
				Width = wallpaper.Width,
				Height = wallpaper.Height,
				Url = wallpaper.Url,
				Source = wallpaper.Source,
				AccountId = AccountId,
				Md5 = wallpaper.Md5,
				ShortUrl = ""
			});

			response.Message = "壁纸上传成功";
			response.IsSuccess = true;
			response.Model = new { Id = wallpaperResult.Item1, Url = Appsetting.FileUrl + wallpaperResult.Item2 };
			return Json(response);
		}

		#endregion

		#region 网络壁纸

		/// <summary>
		/// 网络壁纸
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> WebWallpaper(String webUrl)
		{
			#region 参数验证
			new Parameter().Validate(webUrl);
			#endregion

			var response = new ResponseModel<Tuple<Int32, String>>();

			var result = _wallpaperServices.AddWebWallpaperAsync(AccountId, webUrl);
			response.IsSuccess = true;
			response.Message = "网络壁纸保存成功";
			response.Model = await result;

			return Json(response);
		}

		#endregion

		#region 更换皮肤

		/// <summary>
		/// 更换皮肤
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifySkin(String skin)
		{
			#region 参数验证
			new Parameter().Validate(skin);
			#endregion

			var response = new ResponseModel();

			await _skinServices.ModifySkinAsync(AccountId, skin);
			response.IsSuccess = true;
			response.Message = "更换皮肤成功";

			return Json(response);
		}

		#endregion

		#region 载入上传壁纸

		/// <summary>
		/// 载入上传壁纸
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetUploadWallPapers()
		{
			var response = new ResponseModel<IList<WallpaperDto>>();
			var result = await _wallpaperServices.GetUploadWallpaperAsync(AccountId);
			response.IsSuccess = true;
			response.Message = "载入之前上传的壁纸成功";
			response.Model = result;

			return Json(response);
		}

		#endregion

		#region 获取全部皮肤

		/// <summary>
		/// 获取全部皮肤
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetSkins()
		{
			var response = new ResponseModel<dynamic>();

			var skinPath = "";
			var result = _skinServices.GetAllSkinAsync(skinPath);
			response.IsSuccess = true;
			response.Message = "获取皮肤列表成功";
			response.Model = new { result, currentSkin = (await _accountServices.GetConfigAsync(AccountId)).Skin };

			return Json(response);

		}

		#endregion

		#region 更改码头位置

		/// <summary>
		/// 更改码头位置
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyDockPosition(String pos, Int32 deskNum)
		{
			#region 参数验证
			new Parameter().Validate(pos).Validate(deskNum);
			#endregion

			var response = new ResponseModel();
			await _deskServices.ModifyDockPositionAsync(AccountId, deskNum, pos);
			response.IsSuccess = true;
			response.Message = "更改码头的位置成功";

			return Json(response);
		}

		#endregion

		#region 修改壁纸来源

		/// <summary>
		/// 修改壁纸来源
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyWallpaperSource(String source)
		{
			#region 参数验证
			new Parameter().Validate(source);
			#endregion

			var response = new ResponseModel();
			await _deskServices.ModifyWallpaperSourceAsync(source, AccountId);
			response.IsSuccess = true;
			response.Message = "更改壁纸来源成功";

			return Json(response);
		}

		#endregion

		#region 更改图标大小

		/// <summary>
		/// 更改图标大小
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifySize(Int32 appSize)
		{
			#region 参数验证
			new Parameter().Validate(appSize);
			#endregion

			var response = new ResponseModel();
			await _appServices.ModifyAppIconSizeAsync(AccountId, appSize);
			response.IsSuccess = true;
			response.Message = "更改图标大小成功";

			return Json(response);
		}

		#endregion

		#region 更换默认显示桌面

		/// <summary>
		/// 更换默认显示桌面
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyDefaultDesk(Int32 deskNum)
		{
			#region 参数验证
			new Parameter().Validate(deskNum);
			#endregion

			var response = new ResponseModel();
			await _deskServices.ModifyDefaultDeskNumberAsync(AccountId, deskNum);
			response.IsSuccess = true;
			response.Message = "更换默认桌面成功";

			return Json(response);
		}

		#endregion

		#region 更换图标排列方向

		/// <summary>
		/// 更换图标排列方向
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyXy(String appXy)
		{
			#region 参数验证
			new Parameter().Validate(appXy);
			#endregion

			var response = new ResponseModel();

			await _appServices.ModifyAppDirectionAsync(AccountId, appXy);
			response.IsSuccess = true;
			response.Message = "更换图标排列方向成功";

			return Json(response);
		}

		#endregion

		#region 设置壁纸显示模式

		/// <summary>
		/// 设置壁纸显示模式
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyDisplayModel(String wallPaperShowType)
		{
			#region 参数验证
			new Parameter().Validate(wallPaperShowType);
			#endregion

			var response = new ResponseModel();
			await _wallpaperServices.ModifyWallpaperModeAsync(AccountId, wallPaperShowType);
			response.IsSuccess = true;
			response.Message = "壁纸显示模式设置成功";

			return Json(response);
		}

		#endregion

		#region 更改图标的水平间距

		/// <summary>
		/// 更改图标的水平间距
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyHorizontalSpace(Int32 appHorizontal)
		{
			#region 参数验证
			new Parameter().Validate(appHorizontal);
			#endregion

			var response = new ResponseModel();
			await _appServices.ModifyAppHorizontalSpacingAsync(AccountId, appHorizontal);
			response.IsSuccess = true;
			response.Message = "更改图标水平间距成功";

			return Json(response);
		}

		#endregion

		#region 更改应用图标垂直间距

		/// <summary>
		/// 更改应用图标垂直间距
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ModifyVerticalSpace(Int32 appVertical)
		{
			#region 参数验证
			new Parameter().Validate(appVertical);
			#endregion

			var response = new ResponseModel();
			await _appServices.ModifyAppVerticalSpacingAsync(AccountId, appVertical);
			response.IsSuccess = true;
			response.Message = "更改图标垂直间距成功";

			return Json(response);
		}

		#endregion

	}
}