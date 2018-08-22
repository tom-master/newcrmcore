using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Web.Controllers
{
    public class AppManagerController : BaseController
    {
        private readonly IAppServices _appServices;
        private readonly IAccountServices _accountServices;

        public AppManagerController(IAppServices appServices, IAccountServices accountServices)
        {
            _appServices = appServices;
            _accountServices = accountServices;
        }

        #region 页面

        /// <summary>
        /// 首页
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync(AccountId);
            ViewData["AppStyles"] = _appServices.GetAppStyles().ToList();
            ViewData["AppStates"] = _appServices.GetAppStates().Where(w => w.Name == "未审核" || w.Name == "已发布").ToList();

            return View();
        }

        /// <summary>
        /// app审核
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AppAudit(Int32 appId)
        {
            AppDto appResult = null;
            if (appId != 0)// 如果appId为0则是新创建app
            {
                appResult = await _appServices.GetAppAsync(appId, AccountId);
                ViewData["AppState"] = appResult.AppAuditState;
            }

            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync(AccountId);
            return View(appResult);
        }

        #endregion

        #region 审核通过

        /// <summary>
        /// 审核通过
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Pass(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            var response = new ResponseModel();
            await _appServices.PassAsync(appId);
            response.IsSuccess = true;
            response.Message = "app审核通过";

            return Json(response);
        }

        #endregion

        #region 移除应用

        /// <summary>
        /// 删除app
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Remove(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            var response = new ResponseModel();
            await _appServices.RemoveAppAsync(appId);
            response.IsSuccess = true;
            response.Message = "删除app成功";

            return Json(response);
        }

        #endregion

        #region 审核不通过

        /// <summary>
        /// 审核不通过
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Deny(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            var response = new ResponseModel();
            await _appServices.DenyAsync(appId);
            response.IsSuccess = true;
            response.Message = "app审核不通过";

            return Json(response);
        }

        #endregion

        #region 获取所有应用

        /// <summary>
        /// 获取所有应用
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetApps(String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize)
        {
            var response = new ResponseModels<IList<AppDto>>();
            var result = await _appServices.GetAccountAppsAsync(0, searchText, appTypeId, appStyleId, appState, pageIndex, pageSize);
            foreach (var appDto in result.Models)
            {
                appDto.IsCreater = appDto.AccountId == AccountId;
            }

            response.TotalCount = result.TotalCount;
            response.IsSuccess = true;
            response.Message = "获取app列表成功";
            response.Model = result.Models;

            return Json(response);
        }

        #endregion

        #region 检查应用名称

        /// <summary>
        /// 检查应用名称
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckName(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _accountServices.CheckAppNameAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用名称已存在" });
        }

        #endregion

        #region 检查应用地址

        /// <summary>
        /// 检查应用Url
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckUrl(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _accountServices.CheckAppUrlAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用Url已存在" });
        }

        #endregion

        #region 设置今日推荐应用

        /// <summary>
        /// 设置应用为今日推荐
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Recommend(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            var response = new ResponseModel();
            await _appServices.SetTodayRecommandAppAsync(appId);
            response.IsSuccess = true;
            response.Message = "设置成功";

            return Json(response);
        }

        #endregion
    }
}