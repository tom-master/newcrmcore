using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
    public class AppManagerController : BaseController
    {
        private readonly IAppServices _appServices;

        private readonly IUserServices _userServices;

        public AppManagerController(IAppServices appServices, IUserServices userServices)
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
            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync();
            ViewData["AppStyles"] = _appServices.GetAppStyles().ToList();
            ViewData["AppStates"] = _appServices.GetAppStates().Where(w => w.Name == "未审核" || w.Name == "已发布").ToList();

            return View();
        }

        /// <summary>
        /// app审核
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AppAudit(Int32 appId)
        {
            AppDto appResult = null;
            if (appId != 0)// 如果appId为0则是新创建app
            {
                appResult = await _appServices.GetAppAsync(appId, UserId);
                ViewData["AppState"] = appResult.AppAuditState;
            }

            ViewData["AppTypes"] = await _appServices.GetAppTypesAsync();
            return View(appResult);
        }

        #endregion

        #region 审核通过

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Pass(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            await _appServices.PassAsync(appId);
            return Json(new ResponseModel
            {
                IsSuccess = true,
                Message = "app审核通过"
            });
        }

        #endregion

        #region 移除应用

        /// <summary>
        /// 移除应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Remove(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            await _appServices.RemoveAppAsync(appId);

            return Json(new ResponseModel
            {
                IsSuccess = true,
                Message = "删除app成功"
            });
        }

        #endregion

        #region 审核不通过

        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
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
        /// <param name="searchText"></param>
        /// <param name="appTypeId"></param>
        /// <param name="appStyleId"></param>
        /// <param name="appState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetApps(String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.Validate(searchText, true);

            var response = new ResponseModels<IList<AppDto>>();
            var result = await _appServices.GetUserAppsAsync(0, searchText, appTypeId, appStyleId, appState, pageIndex, pageSize);
            foreach (var appDto in result.Models)
            {
                appDto.IsCreater = appDto.UserId == UserId;
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
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckName(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckAppNameAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用名称已存在" });
        }

        #endregion

        #region 检查应用地址

        /// <summary>
        /// 检查应用Url
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckUrl(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckAppUrlAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用Url已存在" });
        }

        #endregion

        #region 设置今日推荐应用

        /// <summary>
        /// 设置应用为今日推荐
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
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