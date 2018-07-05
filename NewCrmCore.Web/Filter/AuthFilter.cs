using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using Newtonsoft.Json;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

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

			if (filterContext.HttpContext.Request.Method.ToLower() == "post")
			{
				if (!await ValidateToken(filterContext))
				{
					ReturnMessage(filterContext, "不要重复提交表单!");
					return;
				}
			}

			if (filterContext.HttpContext.Request.Query["type"] == "folder")
			{
				return;
			}

			var account = await ((IAccountServices)filterContext.HttpContext.RequestServices.GetService(typeof(IAccountServices)))
				.GetAccountAsync(Int32.Parse(filterContext.HttpContext.Request.Cookies["memberID"]));

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
					ContentType = "utf8"
				};
			}
			else
			{
				filterContext.Result = new ContentResult()
				{
					ContentType = "utf8",
					Content = @"<script>(function(){top.NewCrm.msgbox.fail('" + response.Message + "');})()</script>"
				};
			}
		}

		private async Task<Boolean> ValidateToken(AuthorizationFilterContext filterContext)
		{ 
			//var accountId = Int32.Parse(JsonConvert.DeserializeObject<dynamic>(filterContext.HttpContext.Request.Cookies["Account"]).Id.ToString());
			//var token1 = await CacheHelper.GetOrSetCache<String>(new GlobalUniqueTokenCacheKey(accountId));
			//var token2 = filterContext.HttpContext.Request.Form["hidden_code"];
			//if (token1 != token2)
			//{
			//	return false;
			//}
			//await CacheHelper.RemoveKeyWhenModify(new GlobalUniqueTokenCacheKey(accountId));
			return true;
		}
	}
}