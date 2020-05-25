using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using Newtonsoft.Json;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.WebApi.ControllerHelper
{

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

        protected IActionResult Json(Object model)
        {
            var m = model as ResponseBase;
            if (m != null)
            {
                if (m.IsSuccess)
                {
                    return Ok(model);
                }
                return BadRequest(model);
            }
            throw new BusinessException($@"响应中模型的类型必须为:{nameof(ResponseBase)}类型或派生类型");
        }
    }
}
