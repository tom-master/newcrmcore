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
                var user = ParseUser();
                if (user != null)
                {
                    var userId = user.Id;
                    return Int32.Parse(userId.ToString());
                }
                return 0;
            }
        }

        protected Boolean IsAdmin
        {
            get
            {
                var user = ParseUser();
                if (user != null)
                {
                    return user.IsAdmin;
                }
                return false;
            }
        }

        private UserDto ParseUser()
        {
            var user = Request.Cookies["User"];
            if (user != null)
            {
                return JsonConvert.DeserializeObject<UserDto>(user);
            }
            return null;
        }
    }
}
