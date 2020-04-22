using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace NewCrmCore.Web.Controllers
{
    //[Authorize]
    public class BaseController : Controller
    {
        public UserDto UserInfo
        {
            get
            {
                var userInfo = Request.Cookies["User"];
                if (userInfo != null)
                {
                    return JsonConvert.DeserializeObject<UserDto>(userInfo);
                }
                return null;
            }
        }

        internal void InternalLogout()
        {
            Response.Cookies.Append("User", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
        }
    }
}
