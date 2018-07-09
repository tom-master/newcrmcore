using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
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
	public class WallpaperServices: IWallpaperServices
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
				AccountId = s.AccountId,
				Height = s.Height,
				Id = s.Id,
				Md5 = s.Md5,
				ShortUrl = s.ShortUrl,
				Source = (Int32)s.Source,
				Title = s.Title,
				Url = s.Url,
				Width = s.Width
			}).ToList();
		}

		public async Task<Tuple<Int32, String>> AddWallpaperAsync(WallpaperDto wallpaperDto)
		{
			new Parameter().Validate(wallpaperDto);
			return await _wallpaperContext.AddWallpaperAsync(wallpaperDto.ConvertToModel<WallpaperDto, Wallpaper>());
		}

		public async Task<List<WallpaperDto>> GetUploadWallpaperAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			return (await _wallpaperContext.GetUploadWallpaperAsync(accountId)).Select(s => new WallpaperDto
			{
				AccountId = s.AccountId,
				Height = s.Height,
				Id = s.Id,
				Md5 = s.Md5,
				ShortUrl = s.ShortUrl,
				Source = (Int32)s.Source,
				Title = s.Title,
				Url = Appsetting.FileUrl + s.Url,
				Width = s.Width
			}).ToList();
		}

		public async Task<Tuple<Int32, String>> AddWebWallpaperAsync(Int32 accountId, String url)
		{
			new Parameter().Validate(accountId).Validate(url);

			var imageTitle = Path.GetFileNameWithoutExtension(url);
			Image image;

			using (var stream = await new HttpClient().GetStreamAsync(new Uri(url)))
			using (image = Image.FromStream(stream))
			{
				var md5 = FileHelper.GetMD5(stream);
				var webWallpaper = await GetUploadWallpaperAsync(md5);
				if (webWallpaper != null)
				{
					return new Tuple<Int32, String>(webWallpaper.Id, webWallpaper.ShortUrl);
				}

				var result = await AddWallpaperAsync(new WallpaperDto
				{
					Width = image.Width,
					Height = image.Height,
					Source = (Int32)WallpaperSource.Web,
					Title = imageTitle,
					Url = url,
					AccountId = accountId,
					Md5 = md5,
					ShortUrl = url
				});
				return new Tuple<Int32, String>(result.Item1, result.Item2);
			}
		}

		public async Task<WallpaperDto> GetUploadWallpaperAsync(String md5)
		{
			new Parameter().Validate(md5);
			var result = await _wallpaperContext.GetUploadWallpaperAsync(md5);
			return new WallpaperDto
			{
				AccountId = result.AccountId,
				Height = result.Height,
				Width = result.Width,
				Id = result.Id,
				Md5 = result.Md5,
				ShortUrl = result.ShortUrl,
				Source = (Int32)result.Source,
				Title = result.Title,
				Url = result.Url
			};
		}

		public async Task ModifyWallpaperModeAsync(Int32 accountId, String newMode)
		{
			new Parameter().Validate(accountId).Validate(newMode);
			await _wallpaperContext.ModifyWallpaperModeAsync(accountId, newMode);
			await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task ModifyWallpaperAsync(Int32 accountId, Int32 newWallpaperId)
		{
			new Parameter().Validate(accountId).Validate(newWallpaperId);
			await _wallpaperContext.ModifyWallpaperAsync(accountId, newWallpaperId);
			await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task RemoveWallpaperAsync(Int32 accountId, Int32 wallpaperId)
		{
			new Parameter().Validate(accountId).Validate(wallpaperId);
			await _wallpaperContext.RemoveWallpaperAsync(accountId, wallpaperId);
		}
	}
}
