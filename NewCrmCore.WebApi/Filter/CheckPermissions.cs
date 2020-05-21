using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace NewCrmCore.Web.Filter
{
    public class CheckPermissions : IAsyncAuthorizationFilter
    {
        private readonly IUserServices _userServices;
        private readonly ISecurityServices _securityServices;

        public CheckPermissions(IUserServices userServices, ISecurityServices securityServices)
        {
            _userServices = userServices;
            _securityServices = securityServices;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            var methodInfo = ((ControllerActionDescriptor)filterContext.ActionDescriptor).MethodInfo;
            var isAllowAnonymous = methodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Any();
            if (isAllowAnonymous || filterContext.HttpContext.Request.Query["type"] == "folder")
            {
                return;
            }
            var cookieUser = filterContext.HttpContext.Request.Cookies["User"];
            if (cookieUser == null)
            {
                ReturnMessage(filterContext, "会话过期,请刷新页面后重新登陆");
                return;
            }

            if (!String.IsNullOrEmpty(filterContext.HttpContext.Request.Query["id"]))//appId
            {
                var userId = JsonConvert.DeserializeObject<UserDto>(cookieUser).Id;

                var user = await _userServices.GetUserAsync(userId);
                var appId = Int32.Parse(filterContext.HttpContext.Request.Query["id"]);
                var isPermission = await _securityServices.CheckPermissionsAsync(appId, user.Roles.Select(role => role.Id).ToArray());

                if (!isPermission)
                {
                    ReturnMessage(filterContext, "对不起,您没有访问的权限!");
                }
            }
        }

        private void ReturnMessage(AuthorizationFilterContext filterContext, String message)
        {
            var response = new ResponseSimple
            {
                IsSuccess = false,
                Message = message
            };
            var responseCode = StatusCodes.Status419AuthenticationTimeout;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
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
                    ContentType = "utf8",
                    Content = @"<script>(function(){top.NewCrm.fail('" + response.Message + "');})()</script>"
                };
            }
        }
    }
}