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
