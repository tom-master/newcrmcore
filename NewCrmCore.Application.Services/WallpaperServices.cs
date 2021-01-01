using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;
using NewCrmCore.Infrastructure;

namespace NewCrmCore.Application.Services
{
    public class WallpaperServices : IWallpaperServices
    {
        private readonly IWallpaperContext _wallpaperContext;
        public WallpaperServices(IWallpaperContext wallpaperContext)
        {
            _wallpaperContext = wallpaperContext;
        }

        public async Task<List<WallpaperDto>> GetWallpapersAsync()
        {
            var result = await _wallpaperContext.GetWallpapersAsync();
            return result.Select(s => new WallpaperDto
            {
                UserId = s.UserId,
                Height = s.Height,
                Id = s.Id,
                Md5 = s.Md5,
                ShortUrl = s.ShortUrl,
                Source = EnumExtensions.ToEnum<WallpaperSource>((Int32)s.Source),
                Title = s.Title,
                Url = s.Url,
                Width = s.Width
            }).ToList();
        }

        public async Task<(Int32 wapperId, String url)> AddWallpaperAsync(WallpaperDto wallpaperDto)
        {
            Check.IfNullOrZero(wallpaperDto);
            return await _wallpaperContext.AddWallpaperAsync(wallpaperDto.ConvertToModel<WallpaperDto, Wallpaper>());
        }

        public async Task<List<WallpaperDto>> GetUploadWallpaperAsync(Int32 userId)
        {
            Check.IfNullOrZero(userId);
            var result = await _wallpaperContext.GetUploadWallpaperAsync(userId);
            return result.Select(s => new WallpaperDto
            {
                UserId = s.UserId,
                Height = s.Height,
                Id = s.Id,
                Md5 = s.Md5,
                ShortUrl = s.ShortUrl,
                Source = EnumExtensions.ToEnum<WallpaperSource>((Int32)s.Source),
                Title = s.Title,
                Url = EnumExtensions.ToEnum<WallpaperSource>((Int32)s.Source) == WallpaperSource.Upload ? Appsetting.FileUrl + s.Url : s.Url,
                Width = s.Width
            }).ToList();
        }

        public async Task<(Int32 wapperId, String url)> AddWebWallpaperAsync(Int32 userId, String url)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(url);

            using var stream = await new HttpClient().GetStreamAsync(new Uri(url));
            using Image image = Image.FromStream(stream);
            var md5 = MD5.Get(stream);
            var webWallpaper = await GetUploadWallpaperAsync(md5);
            if (webWallpaper != null)
            {
                return (webWallpaper.Id, webWallpaper.ShortUrl);
            }

            var result = await AddWallpaperAsync(new WallpaperDto
            {
                Width = image.Width,
                Height = image.Height,
                Source = EnumExtensions.ToEnum<WallpaperSource>((Int32)WallpaperSource.Web),
                Title = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5),
                Url = url,
                UserId = userId,
                Md5 = md5,
                ShortUrl = url
            });
            return (result.wapperId, result.url);
        }

        public async Task<WallpaperDto> GetUploadWallpaperAsync(String md5)
        {
            Check.IfNullOrZero(md5);
            var result = await _wallpaperContext.GetUploadWallpaperAsync(md5);
            return result == null ? null : new WallpaperDto
            {
                UserId = result.UserId,
                Height = result.Height,
                Width = result.Width,
                Id = result.Id,
                Md5 = result.Md5,
                ShortUrl = result.ShortUrl,
                Source = EnumExtensions.ToEnum<WallpaperSource>((Int32)result.Source),
                Title = result.Title,
                Url = result.Url
            };
        }

        public async Task ModifyWallpaperModeAsync(Int32 userId, String newMode)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(newMode);
            await _wallpaperContext.ModifyWallpaperModeAsync(userId, newMode);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task ModifyWallpaperAsync(Int32 userId, Int32 newWallpaperId)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(newWallpaperId);
            await _wallpaperContext.ModifyWallpaperAsync(userId, newWallpaperId);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId), new WallpaperCacheKey(userId));
        }

        public async Task RemoveWallpaperAsync(Int32 userId, Int32 wallpaperId)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(wallpaperId);
            await _wallpaperContext.RemoveWallpaperAsync(userId, wallpaperId);
        }
    }
}
