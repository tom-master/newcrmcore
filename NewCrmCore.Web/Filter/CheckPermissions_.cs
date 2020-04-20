using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Web.Controllers;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Filter
{
    public class CheckPermissions_ : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            if (((ControllerActionDescriptor)filterContext.ActionDescriptor).MethodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(AllowAnonymousAttribute)))
            {
                return;
            }

            if (filterContext.HttpContext.Request.Cookies["User"] == null)
            {
                ReturnMessage(filterContext, "会话过期,请刷新页面后重新登陆");
                return;
            }

            if (filterContext.HttpContext.Request.Query["type"] == "folder")
            {
                return;
            }

            if (!String.IsNullOrEmpty(filterContext.HttpContext.Request.Query["id"]))//appId
            {
                var userId = JsonConvert.DeserializeObject<UserDto>(filterContext.HttpContext.Request.Cookies["User"]).Id;

                var user = await ((IUserServices)filterContext
                    .HttpContext
                    .RequestServices
                    .GetService(typeof(IUserServices)))
                    .GetUserAsync(userId);

                var appId = Int32.Parse(filterContext.HttpContext.Request.Query["id"]);
                var isPermission = await ((ISecurityServices)filterContext.HttpContext.RequestServices.GetService(typeof(ISecurityServices))).CheckPermissionsAsync(appId, user.Roles.Select(role => role.Id).ToArray());

                if (!isPermission)
                {
                    ReturnMessage(filterContext, "对不起,您没有访问的权限!");
                }
            }
        }

        private void ReturnMessage(AuthorizationFilterContext filterContext, String message)
        {
            var response = new ResponseModel
            {
                IsSuccess = false,
                Message = message
            };

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult(response);
            }
            else
            {
                filterContext.Result = new ContentResult()
                {
                    ContentType = "utf8",
                    Content = @"<script>(function(){top.NewCrm.fail('" + response.Message + "');})()</script>"
                };
            }
        }
    }
}