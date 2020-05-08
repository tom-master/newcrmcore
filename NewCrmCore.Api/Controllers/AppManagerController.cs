using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewLibCore.Validate;

namespace NewCrmCore.Api.Controllers
{
    public class AppManagerController : NewCrmController
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
        public async Task<IActionResult> InitAppManagerAsync()
        {
            var appTypes = await _appServices.GetAppTypesAsync();
            var appStyles = _appServices.GetAppStyles().ToList();
            var appStates = _appServices.GetAppStates().Where(w => w.Name == "未审核" || w.Name == "已发布").ToList();

            return Json(new ResponseModel<dynamic>
            {
                Model = new { appTypes, appStyles, appStates },
                IsSuccess = true,
                Message = "应用管理初始化成功"
            });
        }

        /// <summary>
        /// app审核
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitAppAuditAsync(Int32 appId)
        {
            AppDto app = null;
            var response = new ResponseModel<dynamic>();
            if (appId != 0)// 如果appId为0则是新创建app
            {
                app = await _appServices.GetAppAsync(appId, UserInfo.Id);
                if (app == null)
                {
                    response.IsSuccess = false;
                    response.Message = "没有获取到指定的应用";
                    return Json(response);
                }
            }
            var uniqueToken = CreateUniqueTokenAsync(UserInfo.Id);
            var appTypes = await _appServices.GetAppTypesAsync();

            response.Model = new { app, uniqueToken, appTypes };
            response.IsSuccess = true;
            response.Message = "应用审核初始化成功";

            return Json(response);
        }

        #endregion

        #region 审核通过

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PassAsync(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            await _appServices.PassAsync(appId);
            return Json(new ResponseSimple
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
        public async Task<IActionResult> RemoveAsync(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            await _appServices.RemoveAppAsync(appId);

            return Json(new ResponseSimple
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
        public async Task<IActionResult> DenyAsync(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            await _appServices.DenyAsync(appId);

            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "app审核不通过"
            });
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
        public async Task<IActionResult> GetAppsAsync(String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.Validate(searchText, true);

            var result = await _appServices.GetUserAppsAsync(0, searchText, appTypeId, appStyleId, appState, pageIndex, pageSize);
            foreach (var appDto in result.Models)
            {
                appDto.IsCreater = appDto.UserId == UserInfo.Id;
            }

            return Json(new ResponsePaging<IList<AppDto>>
            {
                TotalCount = result.TotalCount,
                IsSuccess = true,
                Message = "获取app列表成功",
                Model = result.Models
            });
        }

        #endregion

        #region 检查应用名称

        /// <summary>
        /// 检查应用名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckNameAsync(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckAppNameAsync(param);
            var response = new ResponseSimple();
            if (!result)
            {
                response.Model = "y";
                response.IsSuccess = true;
                response.Message = "";
            }
            else
            {
                response.Model = "n";
                response.IsSuccess = false;
                response.Message = "应用名称已存在";
            }
            return Json(response);
        }

        #endregion

        #region 检查应用地址

        /// <summary>
        /// 检查应用Url
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckUrlAsync(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _userServices.CheckAppUrlAsync(param);
            var response = new ResponseSimple();
            if (!result)
            {
                response.Model = "y";
                response.IsSuccess = true;
                response.Message = "";
            }
            else
            {
                response.Model = "n";
                response.IsSuccess = false;
                response.Message = "应用Url已存在";
            }
            return Json(response);
        }

        #endregion

        #region 设置今日推荐应用

        /// <summary>
        /// 设置应用为今日推荐
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RecommendAsync(Int32 appId)
        {
            #region 参数验证	
            Parameter.Validate(appId);
            #endregion

            await _appServices.SetTodayRecommandAppAsync(appId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "设置成功"
            });
        }

        #endregion
    }
}