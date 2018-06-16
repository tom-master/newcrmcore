﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Web.Controllers.ControllerHelper;

namespace NewCrmCore.Web.Filter
{
	public class AuthFilter: IAsyncAuthorizationFilter
	{

		public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
		{

			if (!ValidateToken(filterContext))
			{
				ReturnMessage(filterContext, "token验证失败！");
				return;
			}

			if (filterContext.Filters.Where(w => w.GetType() == typeof(DoNotCheckPermissionAttribute)).Any())
			{
				return;
			}

			//文件夹
			if (filterContext.HttpContext.Request.Query["type"] == "folder")
			{
				return;
			}

			if (filterContext.HttpContext.Request.Cookies["memberID"] == null)
			{
				ReturnMessage(filterContext, "登陆超时，请刷新页面后重新登陆");
				return;
			}

			var account = await ((IAccountServices)filterContext.HttpContext.RequestServices.GetService(typeof(IAccountServices))).GetAccountAsync(Int32.Parse(filterContext.HttpContext.Request.Cookies["memberID"]));

			var appId = Int32.Parse(filterContext.HttpContext.Request.Query["id"]);
			var isPermission = await ((ISecurityServices)filterContext.HttpContext.RequestServices.GetService(typeof(ISecurityServices))).CheckPermissionsAsync(appId, account.Roles.Select(role => role.Id).ToArray());

			if (!isPermission)
			{
				ReturnMessage(filterContext, "对不起，您没有访问的权限！");
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
					StatusCode = 401
				};
			}
		}

		private Boolean ValidateToken(AuthorizationFilterContext filterContext)
		{
			var token = filterContext.RouteData.Values["token"];
			if (token == null || String.IsNullOrEmpty(token.ToString()))
			{
				//return false;
			}

			return true;
		}
	}
}