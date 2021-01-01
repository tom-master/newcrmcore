using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.WebApi.ApiHelper;
using NewLibCore.Validate;

namespace NewCrmCore.WebApi.Controllers
{
    public class LoggerController : NewCrmController
    {
        private readonly ILoggerServices _loggerServices;

        public LoggerController(ILoggerServices loggerServices)
        {
            _loggerServices = loggerServices;
        }

        #region 页面

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
        public async Task<IActionResult> GetLogsAsync(String userName, Int32 loglevel, Int32 pageIndex, Int32 pageSize)
        {
            Check.IfNullOrZero(userName, true);
            var result = await _loggerServices.GetLogsAsync(userName, loglevel, pageIndex, pageSize);
            return Json(new ResponsePaging<IList<LogDto>>
            {
                IsSuccess = true,
                Message = "获取日志列表成功",
                Model = result.Models,
                TotalCount = result.TotalCount,
            });
        }

        #endregion
    }
}