using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Controllers.ControllerHelper
{
	public class BaseController: Controller
	{
		protected Int32 AccountId
		{
			get
			{
				var account = Request.Cookies["Account"];
				var accountId = JsonConvert.DeserializeObject<dynamic>(account).Id;
				if (accountId != null)
				{
					return Int32.Parse(accountId.ToString());
				}
				return 0;
			}
		}
	}
}
