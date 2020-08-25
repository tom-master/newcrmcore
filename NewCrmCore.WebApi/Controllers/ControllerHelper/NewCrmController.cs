using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.WebApi.ApiHelper
{
    [ApiController, Authorize, Route("api/[controller]/[action]")]
    public class NewCrmController : ControllerBase
    {
        protected async Task<UserDto> GetUserContextAsync()
        {
            var auth = await HttpContext.AuthenticateAsync().ConfigureAwait(false);
            var claims = auth.Principal.Claims.ToList();
            var userId = claims.FirstOrDefault(w => w.Type == "UserId")?.Value;
            var isAdmin = claims.FirstOrDefault(w => w.Type == "IsAdmin")?.Value;
            return new UserDto
            {
                UserFace = claims.FirstOrDefault(w => w.Type == "UserFace")?.Value,
                Name = claims.FirstOrDefault(w => w.Type == "UserName")?.Value,
                Id = userId == null ? 0 : Int32.Parse(userId),
                IsAdmin = isAdmin != null && Boolean.Parse(isAdmin)
            };
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

            if (model is ResponseBase m)
            {
                m.HttpStatusCode = HttpContext.Response.StatusCode;
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
