using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;

namespace NewCrmCore.Web.Controllers.ControllerHelper
{
	public class BaseController : Controller
	{
		protected Int32 AccountId
		{
			get
			{
				var accountId = Request.Cookies["memberID"];

				if (accountId != null)
				{
					return Int32.Parse(accountId.ToString());
				}
				return 0;
			}
		} 
	}
}
