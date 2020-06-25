using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Filter
{
    public sealed class HandleException : IExceptionFilter
    {
        private readonly ILoggerServices _loggerServices;
        public HandleException(ILoggerServices loggerServices)
        {
            _loggerServices = loggerServices;
        }

        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            var businessException = filterContext.Exception is BusinessException;

            var response = new ResponseSimple
            {
                IsSuccess = false,
                Message = businessException ? filterContext.Exception.Message : "出现未知错误，请查看日志",
            };

            var responseCode = StatusCodes.Status500InternalServerError;
            var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
            if (isAjaxRequest)
            {
                filterContext.Result = new JsonResult(response)
                {
                    ContentType = "UTF8",
                    StatusCode = responseCode
                };
            }
            else
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = responseCode,
                    ContentType = "UTF8",
                    Content = @"<script>(function(){top.NewCrm.fail('" + response.Message + "');})()</script>"
                };
            }

            var userId = 0;
            var userCookie = filterContext.HttpContext.Request.Cookies["User"];
            if (userCookie != null)
            {
                userId = JsonConvert.DeserializeObject<UserDto>(userCookie).Id;
            }

            _loggerServices.AddLoggerAsync(new LogDto
            {
                Action = filterContext.RouteData.Values["action"].ToString(),
                Controller = filterContext.RouteData.Values["controller"].ToString(),
                ExceptionMessage = filterContext.Exception.Message,
                Track = filterContext.Exception.StackTrace,
                LogLevelEnum = businessException ? Domain.ValueObject.LogLevel.Error : Domain.ValueObject.LogLevel.Exception,
                AddTime = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                UserId = userId
            });
        }
    }

    public static class ErrorFilterExtension
    {
        public static Boolean IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
            return false;
        }
    }
}