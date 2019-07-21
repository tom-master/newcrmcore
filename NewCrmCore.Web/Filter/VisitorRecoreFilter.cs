using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Filter
{
    public class VisitorRecordFilter : ActionFilterAttribute, IAsyncActionFilter
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userCookie = context.HttpContext.Request.Cookies["User"];
            if (!String.IsNullOrEmpty(userCookie))
            {
                var user = JsonConvert.DeserializeObject<UserDto>(userCookie);
                await ((ISecurityServices)context.HttpContext.RequestServices.GetService(typeof(ISecurityServices))).AddVisitorRecord(new VisitorRecordDto
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    Action = context.RouteData.Values["action"].ToString(),
                    Controller = context.RouteData.Values["controller"].ToString(),
                    Ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    VisitorUrl = GetCompliteUrl(context.HttpContext.Request),
                    UrlParameter = JsonConvert.SerializeObject(context.ActionArguments)
                });
            }
            await next();
        }

        private String GetCompliteUrl(HttpRequest httpRequest)
        {
            return $@"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}{httpRequest.Path}?{httpRequest.QueryString}";
        }
    }
}