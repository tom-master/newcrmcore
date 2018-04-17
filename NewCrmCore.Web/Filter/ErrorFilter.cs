using System;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools.CustomException;

namespace NewCRM.Web.Filter
{
	public sealed class ErrorFilter: IExceptionFilter
	{
		public void OnException(ExceptionContext filterContext)
		{
			filterContext.ExceptionHandled = true;

			var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
			var exception = filterContext.Exception is BusinessException;
			if (isAjaxRequest)
			{
				filterContext.Result = new JsonResult(new
				{
					IsSuccess = false,
					Message = exception ? filterContext.Exception.Message : "出现未知错误，请重试",
				})
				{
					ContentType = "utf8",
					StatusCode = 500
				};

				return;
			}

			DependencyResolver.Current.GetService<ILoggerServices>().AddLoggerAsync(new LogDto
			{
				Action = filterContext.RouteData.Values["action"].ToString(),
				Controller = filterContext.RouteData.Values["controller"].ToString(),
				ExceptionMessage = filterContext.Exception.Message,
				Track = filterContext.Exception.StackTrace,
				LogLevelEnum = exception ? LogLevel.Warning : LogLevel.Error,
				Id = new Random().Next(1, Int32.MaxValue),
				AddTime = DateTime.Now.ToString(CultureInfo.CurrentCulture)
			});

			filterContext.Result = new ContentResult
			{
				ContentType = "UTF8",
				Content = $@"<script>top.alertInfo('出现未知错误，请重试')</script>"
			};
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