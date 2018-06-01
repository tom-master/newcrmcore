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

namespace NewCrmCore.Web.Controllers
{
	public class AppTypesController: BaseController
	{
		private readonly IAppServices _appServices;

		public AppTypesController(IAppServices appServices)
		{
			_appServices = appServices;
		}

		#region 类目管理

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
		/// 创建新的类目
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> CreateNewAppType(Int32 appTypeId = 0)
		{
			AppTypeDto result = null;
			if (appTypeId != 0)
			{
				result = (await _appServices.GetAppTypesAsync()).FirstOrDefault(appType => appType.Id == appTypeId);
			}

			return View(result);
		}

		#endregion

		/// <summary>
		/// 获取所有app类型
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> GetAppTypes(Int32 pageIndex, Int32 pageSize, String searchText)
		{
			var response = new ResponseModels<IList<AppTypeDto>>();
			var result = (await _appServices.GetAppTypesAsync()).Where(appType => String.IsNullOrEmpty(searchText) || appType.Name.Contains(searchText)).OrderByDescending(d => d.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
			response.Message = "app类型获取成功";
			response.IsSuccess = true;
			response.Model = result;
			response.TotalCount = result.Count;

			return Json(response);

		}

		/// <summary>
		/// 删除app类型
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> RemoveAppType(Int32 appTypeId)
		{
			#region 参数验证
			new Parameter().Validate(appTypeId);
			#endregion

			var response = new ResponseModel();
			await _appServices.RemoveAppTypeAsync(appTypeId);
			response.IsSuccess = true;
			response.Message = "删除app类型成功";

			return Json(response);
		}

		/// <summary>
		/// 创建新的app类型
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> CreateAppType(IFormCollection forms, Int32 appTypeId = 0)
		{
			#region 参数验证
			new Parameter().Validate(forms);
			#endregion

			var response = new ResponseModel();
			var appTypeDto = WrapperAppTypeDto(forms);
			if (appTypeId == 0)
			{
				await _appServices.CreateNewAppTypeAsync(appTypeDto);
			}
			else
			{
				await _appServices.ModifyAppTypeAsync(appTypeDto, appTypeId);
			}
			response.IsSuccess = true;
			response.Message = "app类型创建成功";

			return Json(response);
		}


		/// <summary>
		/// 检查应用类型名称
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> CheckAppTypeName(String param)
		{
			#region 参数验证
			new Parameter().Validate(param);
			#endregion

			var result = await _appServices.CheckAppTypeNameAsync(param);
			return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "类型名称已存在" });
		}

		#region private method

		/// <summary>
		/// 封装从页面传入的forms表单到AppTypeDto类型
		/// </summary>
		/// <returns></returns>
		private static AppTypeDto WrapperAppTypeDto(IFormCollection forms)
		{
			var appTypeDto = new AppTypeDto
			{
				Name = forms["val_name"]
			};

			if ((forms["id"] + "").Length > 0)
			{
				appTypeDto.Id = Int32.Parse(forms["id"]);
			}

			return appTypeDto;

		}

		#endregion
	}
}