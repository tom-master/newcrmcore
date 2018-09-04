using System;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Controllers.ControllerHelper
{
    public class BaseController : Controller
    {
        protected Int32 UserId
        {
            get
            {
                var user = Request.Cookies["User"];
                if (user != null)
                {
                    var userId = JsonConvert.DeserializeObject<UserDto>(user).Id;
                    return Int32.Parse(userId.ToString());
                }
                return 0;
            }
        }
    }
}
