using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public UserDto UserInfo
        {
            get
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
}
