using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Filter
{
    public sealed class ErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
            var exception = filterContext.Exception is BusinessException;

            var response = new ResponseModel
            {
                IsSuccess = false,
                Message = exception ? filterContext.Exception.Message : "出现未知错误，请重试",
            };


            if (isAjaxRequest)
            {
                filterContext.Result = new JsonResult(response);
            }
            else
            {
                filterContext.Result = new ContentResult()
                {
                    ContentType = "UTF8",
                    Content = @"<script>(function(){top.NewCrm.msgbox.fail('" + response.Message + "');})()</script>"
                };
            }
            var userId = JsonConvert.DeserializeObject<UserDto>(filterContext.HttpContext.Request.Cookies["User"]).Id;
            ((ILoggerServices)filterContext.HttpContext.RequestServices.GetService(typeof(ILoggerServices))).AddLoggerAsync(new LogDto
            {
                Action = filterContext.RouteData.Values["action"].ToString(),
                Controller = filterContext.RouteData.Values["controller"].ToString(),
                ExceptionMessage = filterContext.Exception.Message,
                Track = filterContext.Exception.StackTrace,
                LogLevelEnum = exception ? Domain.ValueObject.LogLevel.Warning : Domain.ValueObject.LogLevel.Error,
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