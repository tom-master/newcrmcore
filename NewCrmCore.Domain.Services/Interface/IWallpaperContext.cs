using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;

namespace NewCrmCore.Domain.Services.Interface
{
	public interface IWallpaperContext
	{
		/// <summary>
		/// 获取壁纸
		/// </summary>
		Task<List<Wallpaper>> GetWallpapersAsync();

		/// <summary>
		/// 添加壁纸
		/// </summary>
		Task<Tuple<Int32, String>> AddWallpaperAsync(Wallpaper wallpaper);

		/// <summary>
		/// 获取上传的壁纸
		/// </summary>
		Task<List<Wallpaper>> GetUploadWallpaperAsync(Int32 accountId);

		/// <summary>
		/// 获取上传的壁纸
		/// </summary>
		Task<Wallpaper> GetUploadWallpaperAsync(String md5);

		/// <summary>
		/// 修改壁纸的显示模式
		/// </summary>
		Task ModifyWallpaperModeAsync(Int32 accountId, String newMode);

		/// <summary>
		/// 更换壁纸
		/// </summary>
		Task ModifyWallpaperAsync(Int32 accountId, Int32 newWallpaperId);

		/// <summary>
		/// 根据用户id删除壁纸
		/// </summary>
		Task RemoveWallpaperAsync(Int32 accountId, Int32 wallpaperId);
	}
}
