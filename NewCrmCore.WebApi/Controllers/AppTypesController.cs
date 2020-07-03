using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.WebApi.ApiHelper;
using NewLibCore.Validate;

namespace NewCrmCore.WebApi.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AppTypesController : NewCrmController
    {
        private readonly IAppServices _appServices;

        public AppTypesController(IAppServices appServices)
        {
            _appServices = appServices;
        }

        #region 类目管理

        /// <summary>
        /// 创建新的类目
        /// </summary>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitCreateNewAppTypeAsync(Int32 appTypeId = 0)
        {
            AppTypeDto appTypeDto = null;
            var response = new ResponseModel<dynamic>();
            if (appTypeId != 0)
            {
                appTypeDto = (await _appServices.GetAppTypesAsync()).FirstOrDefault(appType => appType.Id == appTypeId);
                if (appTypeDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "未获取到应用类型";
                    return Json(response);
                }
            }
            var uniqueToken = CreateUniqueTokenAsync(UserInfo.Id);
            response.Model = new { appTypeDto, uniqueToken };
            response.IsSuccess = true;
            response.Message = "创建类目初始化成功";
            return Json(response);
        }

        #endregion

        #region 移除应用类型

        /// <summary>
        /// 移除应用类型
        /// </summary>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveAsync(Int32 appTypeId)
        {
            #region 参数验证
            Parameter.Validate(appTypeId);
            #endregion

            await _appServices.RemoveAppTypeAsync(appTypeId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "删除app类型成功"
            });
        }

        #endregion

        #region 创建应用类型

        /// <summary>
        /// 创建应用类型
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(IFormCollection forms, Int32 appTypeId = 0)
        {
            #region 参数验证
            Parameter.Validate(forms);
            #endregion

            var appTypeDto = WrapperAppTypeDto(forms);
            if (appTypeId == 0)
            {
                await _appServices.CreateNewAppTypeAsync(appTypeDto);
            }
            else
            {
                await _appServices.ModifyAppTypeAsync(appTypeDto, appTypeId);
            }

            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "app类型创建成功"
            });
        }

        #endregion

        #region 检查类型名称

        /// <summary>
        /// 检查类型名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckNameAsync(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _appServices.CheckAppTypeNameAsync(param);
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
                response.Message = "类型名称已存在";
            }
            return Json(response);
        }

        #endregion

        #region 获取所有应用类型

        /// <summary>
        /// 获取所有应用类型
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTypesAsync(Int32 pageIndex, Int32 pageSize, String searchText)
        {
            var result = (await _appServices.GetAppTypesAsync()).Where(appType => String.IsNullOrEmpty(searchText) || appType.Name.Contains(searchText)).OrderByDescending(d => d.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return Json(new ResponsePaging<IList<AppTypeDto>>
            {
                Message = "app类型获取成功",
                IsSuccess = true,
                Model = result,
                TotalCount = result.Count
            });
        }

        #endregion

        #region private method

        /// <summary>
        /// 封装从页面传入的forms表单到AppTypeDto类型
        /// </summary>
        /// <param name="forms"></param>
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

            appTypeDto.IsSystem = Int32.Parse(forms["val_issystem"]) == 1;
            return appTypeDto;
        }

        #endregion
    }
}