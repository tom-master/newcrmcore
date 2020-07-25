using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.WebApi.ApiHelper;

namespace NewCrmCore.WebApi.Middleware
{
    public class HandlerGlobalErrorMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public HandlerGlobalErrorMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate.Invoke(context);
            }
            catch (Exception ex)
            {
                var loggerService = context.RequestServices.GetService<ILoggerServices>();
                var businessException = ex is BusinessException;

                var response = new ResponseSimple
                {
                    IsSuccess = false,
                    Message = businessException ? ex.Message : "出现未知错误，请查看日志",
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                await loggerService.AddLoggerAsync(new LogDto
                {
                    Action = context.Request.RouteValues["action"].ToString(),
                    Controller = context.Request.RouteValues["controller"].ToString(),
                    ExceptionMessage = ex.Message,
                    Track = ex.StackTrace,
                    LogLevelEnum = businessException ? Domain.ValueObject.LogLevel.Error : Domain.ValueObject.LogLevel.Exception,
                    AddTime = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                    UserId = 0
                });
            }
        }
    }
}