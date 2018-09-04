using System;
using System.Collections.Generic;
using System.Linq;
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
using NewCrmCore.Domain.ValueObject;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Web.Controllers
{
    public class AppMarketController : BaseController
    {
        private readonly IAppServices _appServices;

        private readonly IUserServices _userServices;

        public AppMarketController(IAppServices appServices, IUserServices userServices)
        {
            _appServices = appServices;
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
            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync(UserId);
            ViewData["TodayRecommendApp"] = await _appServices.GetTodayRecommendAsync(UserId);

            var user = await _userServices.GetUserAsync(UserId);
            ViewData["UserName"] = user.Name;
            ViewData["UserApp"] = await _appServices.GetDevelopAndNotReleaseCountAsync(UserId);
            return View();
        }

        /// <summary>
        /// 应用详情
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AppDetail(Int32 appId)
        {
            #region 参数验证
            Parameter.Validate(appId);
            #endregion

            ViewData["IsInstallApp"] = await _appServices.IsInstallAppAsync(UserId, appId);
            var result = await _appServices.GetAppAsync(appId, UserId);
            ViewData["UserName"] = result.UserName;

            return View(result);
        }

        /// <summary>
        /// 用户应用管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UserAppManage()
        {
            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync(UserId);
            ViewData["AppStyles"] = _appServices.GetAppStyles().ToList();
            ViewData["AppStates"] = _appServices.GetAppStates().ToList();

            return View();
        }

        /// <summary>
        /// 我的应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UserAppManageInfo(Int32 appId)
        {
            AppDto result = null;
            if (appId != 0)// 如果appId为0则是新创建app
            {
                result = await _appServices.GetAppAsync(appId, UserId);
                ViewData["AppState"] = result.AppAuditState;
            }
            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync(UserId);
            ViewData["UserId"] = UserId;
            return View(result);
        }


        #endregion

        #region 应用打分

        /// <summary>
        /// 应用打分
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyStar(ModifyStar model)
        {
            #region 参数验证
            Parameter.Validate(model.AppId);
            Parameter.Validate(model.StarCount);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppStarAsync(UserId, model.AppId, model.StarCount);
            response.IsSuccess = true;
            response.Message = "打分成功";

            return Json(response);
        }

        #endregion

        #region 安装应用

        /// <summary>
        /// 安装应用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Install(Install model)
        {
            #region 参数验证
            Parameter.Validate(model.AppId);
            Parameter.Validate(model.DeskNum);
            #endregion

            var response = new ResponseModel();
            await _appServices.InstallAppAsync(UserId, model.AppId, model.DeskNum);
            response.IsSuccess = true;
            response.Message = "安装成功";

            return Json(response);
        }

        #endregion

        #region 更新图标 

        /// <summary>
        /// 更新图标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyIcon(ModifyIconForApp model)
        {
            #region 参数验证
            Parameter.Validate(model.AppId);
            Parameter.Validate(model.NewIcon);
            #endregion

            var response = new ResponseModel<String>();
            await _appServices.ModifyAppIconAsync(UserId, model.AppId, model.NewIcon);

            response.IsSuccess = true;
            response.Message = "更新图标成功";
            response.Model = Appsetting.FileUrl + model.NewIcon;

            return Json(response);
        }

        #endregion

        #region 创建应用

        /// <summary>
        /// 创建应用
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            var response = new ResponseModel();

            var appDto = WrapperAppDto(forms);
            appDto.UserId = UserId;
            await _appServices.CreateNewAppAsync(appDto);

            response.IsSuccess = true;
            response.Message = "app创建成功";

            return Json(response);
        }

        #endregion

        #region 发布应用

        /// <summary>
        /// 发布应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Release(Int32 appId)
        {
            #region 参数验证
            Parameter.Validate(appId);
            #endregion

            var response = new ResponseModel();
            await _appServices.ReleaseAppAsync(appId);
            response.IsSuccess = true;
            response.Message = "app发布成功";

            return Json(response);
        }

        #endregion

        #region 修改应用信息

        /// <summary>
        /// 修改应用信息
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyInfo(IFormCollection forms)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyUserAppInfoAsync(UserId, WrapperAppDto(forms));
            response.IsSuccess = true;
            response.Message = "修改app信息成功";

            return Json(response);
        }

        #endregion

        #region 获取所有应用

        /// <summary>
        /// 获取所有应用
        /// </summary>
        /// <param name="appTypeId"></param>
        /// <param name="orderId"></param>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetApps(Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize)
        {
            var response = new ResponseModels<IList<AppDto>>();
            var result = await _appServices.GetAppsAsync(UserId, appTypeId, orderId, searchText, pageIndex, pageSize);
            if (result != null)
            {
                response.TotalCount = result.TotalCount;
                response.IsSuccess = true;
                response.Message = "app列表获取成功";
                response.Model = result.Models;
            }
            else
            {
                response.Message = "app列表获取失败";
            }
            return Json(response);
        }

        #endregion

        #region 获取账户下应用

        /// <summary>
        /// 获取账户下应用
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="appTypeId"></param>
        /// <param name="appStyleId"></param>
        /// <param name="appState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserApps(String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize)
        {
            var response = new ResponseModels<IList<AppDto>>();
            var result = await _appServices.GetUserAppsAsync(UserId, searchText, appTypeId, appStyleId, appState, pageIndex, pageSize);
            if (result != null)
            {
                response.TotalCount = result.TotalCount;
                response.IsSuccess = true;
                response.Message = "app列表获取成功";
                response.Model = result.Models;
            }
            else
            {
                response.Message = "app列表获取失败";
            }

            return Json(response);
        }

        #endregion

        #region 移除账户中应用

        /// <summary>
        /// 移除账户中应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Remove(Int32 appId)
        {
            #region 参数验证
            Parameter.Validate(appId);
            #endregion

            var response = new ResponseModel();
            await _appServices.RemoveAppAsync(appId);
            response.IsSuccess = true;
            response.Message = "删除用户开发的app成功";

            return Json(response);
        }

        #endregion

        #region private method

        /// <summary>
        /// 封装从页面传入的forms表单到AppDto类型
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        private static AppDto WrapperAppDto(IFormCollection forms)
        {
            var appDto = new AppDto
            {
                IconUrl = forms["val_icon"],
                Name = forms["val_name"],
                AppTypeId = Int32.Parse(forms["val_app_category_id"]),
                AppUrl = forms["val_url"],
                Width = Int32.Parse(forms["val_width"]),
                Height = Int32.Parse(forms["val_height"]),
                AppStyle = EnumExtensions.ToEnum<AppStyle>(forms["val_type"]),
                Remark = forms["val_remark"],
                AppAuditState = EnumExtensions.ToEnum<AppAuditState>(Int32.Parse(forms["val_verifytype"])),
                AppReleaseState = AppReleaseState.UnRelease, //未发布
                IsIconByUpload = Int32.Parse(forms["isIconByUpload"]) == 1
            };

            if (appDto.AppStyle == AppStyle.App)
            {
                appDto.IsResize = Int32.Parse(forms["val_isresize"]) == 1;
                appDto.IsOpenMax = Int32.Parse(forms["val_isopenmax"]) == 1;
                appDto.IsFlash = Int32.Parse(forms["val_isflash"]) == 1;
                appDto.IsSetbar = Int32.Parse(forms["val_issetbar"]) == 1;
            }

            if ((forms["val_Id"] + "").Length > 0)
            {
                appDto.Id = Int32.Parse(forms["val_Id"]);
            }

            return appDto;
        }

        #endregion
    }
}