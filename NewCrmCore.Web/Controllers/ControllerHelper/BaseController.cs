using System;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
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
				if (account != null)
				{
					var accountId = JsonConvert.DeserializeObject<AccountDto>(account).Id;
					return Int32.Parse(accountId.ToString());
				}
				return 0;
			}
		}
	}
}
