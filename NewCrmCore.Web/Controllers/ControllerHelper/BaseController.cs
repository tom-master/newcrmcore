using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace NewCrmCore.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public UserDto UserInfo
        {
            get
            {
                var authResult = AsyncContext.Run(() => HttpContext.AuthenticateAsync());
                if (authResult != null)
                {
                    var userInfo = authResult.Principal.Claims.FirstOrDefault(w => w.Type == "User").Value;
                    return JsonConvert.DeserializeObject<UserDto>(userInfo);
                }
                return null;
            }
        }
    }
}
