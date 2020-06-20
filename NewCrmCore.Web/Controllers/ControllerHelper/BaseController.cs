using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using Newtonsoft.Json;
using Nito.AsyncEx;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Web.Controllers
{
    //[Authorize]
    public class NewCrmController : ControllerBase
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

        protected void InternalLogout()
        {
            Response.Cookies.Append("User", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
        }

        protected async Task<String> CreateUniqueTokenAsync(Int32 userId)
        {
            var milliseconds = (DateTime.Now - (new DateTime(1970, 1, 1))).TotalMilliseconds.ToString();
            await CacheHelper.GetOrSetCacheAsync(new GlobalUniqueTokenCacheKey(userId.ToString()), async () => await Task.Run(() => milliseconds));
            return milliseconds;
        }

        protected async Task RemoveUniqueTokenAsync(Int32 userId)
        {
            await CacheHelper.RemoveKeyWhenModify(new GlobalUniqueTokenCacheKey(userId.ToString()));
        }
    }
}
