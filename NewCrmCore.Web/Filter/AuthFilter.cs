using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCRM.Web.Filter
{
	public class AuthFilter: IAuthorizationFilter
	{

		public void OnAuthorization(AuthorizationFilterContext filterContext)
		{

			if (ValidateToken(filterContext))
			{
				ReturnMessage(filterContext, "token验证失败！");
				return;
			}

			var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();
			var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
			if ((controllerName == "desktop" && actionName == "login") || actionName == "landing" || actionName == "index")
			{
				return;
			}

			if (filterContext.HttpContext.Request.Cookies["memberID"] == null)
			{
				ReturnMessage(filterContext, "登陆超时，请刷新页面后重新登陆");
				return;
			}

			if (actionName != "createwindow")
			{
				return;
			}
			//文件夹
			if (filterContext.HttpContext.Request.Form["type"] == "folder")
			{
				return;
			}

			var account = AsyncContext.Run(() => DependencyResolver.Current.GetService<IAccountServices>().GetAccountAsync(Int32.Parse(filterContext.HttpContext.Request.Cookies["memberID"].Value)));

			var appId = Int32.Parse(filterContext.HttpContext.Request.Query["id"]);
			var isPermission = AsyncContext.Run(() => DependencyResolver.Current.GetService<ISecurityServices>().CheckPermissionsAsync(appId, account.Roles.Select(role => role.Id).ToArray()));

			if (!isPermission)
			{
				ReturnMessage(filterContext, "对不起，您没有访问的权限！");
			}
		}



		private void ReturnMessage(AuthorizationFilterContext filterContext, String message)
		{
			var notPermissionMessage = $@"<script>top.alertInfo('{message}',1)</script>";
			var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();

			if (!isAjaxRequest)
			{
				filterContext.Result = new ContentResult
				{
					ContentType = "utf8",
					Content = notPermissionMessage
				};
			}
			else
			{
				var response = new ResponseModel<dynamic>
				{
					IsSuccess = false,
					Message = message
				};

				filterContext.Result = new JsonResult(response)
				{
					ContentType = "utf8"
				};
			}
		}

		private Boolean ValidateToken(AuthorizationFilterContext filterContext)
		{
			var token = filterContext.RouteData.Values["token"].ToString();
			if (String.IsNullOrEmpty(token))
			{
				//return false;
			}

			return true;
		}
	}
}