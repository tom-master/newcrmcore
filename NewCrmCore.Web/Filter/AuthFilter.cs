using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Web.Controllers.ControllerHelper;

namespace NewCrmCore.Web.Filter
{
	public class AuthFilter : IAsyncAuthorizationFilter
	{

		public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
		{
			if (filterContext.ActionDescriptor.FilterDescriptors.Any(a => a.Filter is DoNotCheckPermissionAttribute))
			{
				return;
			}

			if (filterContext.HttpContext.Request.Cookies["memberID"] == null)
			{
				ReturnMessage(filterContext, "会话过期,请刷新页面后重新登陆");
				return;
			}

			if (!ValidateToken(filterContext))
			{
				ReturnMessage(filterContext, "不要重复提交表单!");
				return;
			}

			if (filterContext.HttpContext.Request.Query["type"] == "folder")
			{
				return;
			}

			var account = await ((IAccountServices)filterContext.HttpContext.RequestServices.GetService(typeof(IAccountServices))).GetAccountAsync(Int32.Parse(filterContext.HttpContext.Request.Cookies["memberID"]));

			var appId = Int32.Parse(filterContext.HttpContext.Request.Query["id"]);
			var isPermission = await ((ISecurityServices)filterContext.HttpContext.RequestServices.GetService(typeof(ISecurityServices))).CheckPermissionsAsync(appId, account.Roles.Select(role => role.Id).ToArray());

			if (!isPermission)
			{
				ReturnMessage(filterContext, "对不起,您没有访问的权限!");
			}
		}



		private void ReturnMessage(AuthorizationFilterContext filterContext, String message)
		{
			var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();

			var response = new ResponseModel
			{
				IsSuccess = false,
				Message = message
			};

			if (isAjaxRequest)
			{
				filterContext.Result = new JsonResult(response)
				{
					ContentType = "utf8",
					StatusCode = 401
				};
			}
			else
			{
				filterContext.Result = new ContentResult()
				{
					ContentType = "utf8",
					StatusCode = 401,
					Content = @"<script>(function(){top.NewCrm.msgbox.fail('" + response.Message + "');})()</script>"
				};
			}
		}

		private Boolean ValidateToken(AuthorizationFilterContext filterContext)
		{
			if (filterContext.HttpContext.Request.Method.ToLower() == "post")
			{
				var a1 = filterContext.HttpContext.Request.Cookies["token"];
				var b2 = filterContext.HttpContext.Request.Form["hidden_code"];
				if (a1 != b2)
				{
					return false;
				}
				filterContext.HttpContext.Response.Cookies.Append("token", a1, new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
			}
			return true;
		}
	}
}