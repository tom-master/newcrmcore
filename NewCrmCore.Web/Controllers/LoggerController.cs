using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewLibCore.Validate;

namespace NewCrmCore.Web.Controllers
{
    public class LoggerController : BaseController
    {
        private readonly ILoggerServices _loggerServices;

        public LoggerController(ILoggerServices loggerServices)
        {
            _loggerServices = loggerServices;
        }

        #region 页面
        /// <summary>
        /// 首页
        /// </summary> 
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region 获取日志列表

        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="loglevel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLogs(String userName, Int32 loglevel, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.Validate(userName, true);
            var response = new ResponseModels<IList<LogDto>>();
            var result = await _loggerServices.GetLogsAsync(userName, loglevel, pageIndex, pageSize);
            response.IsSuccess = true;
            response.Message = "获取日志列表成功";
            response.Model = result.Models;
            response.TotalCount = result.TotalCount;

            return Json(response);
        }

        #endregion
    }
}