using System;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace NewCrmCore.Web.Controllers
{
    public class BaseController : Controller
    {
        public UserDto UserInfo
        {
            get
            {
                var user = AsyncContext.Run(() => HttpContext.AuthenticateAsync());
                if (user.Principal != null)
                {
                    var r = user.Principal.Claims.FirstOrDefault(w => w.Type == "User").Value;
                    return JsonConvert.DeserializeObject<UserDto>(r);
                }
                return null;
            }
        }
    }
}
