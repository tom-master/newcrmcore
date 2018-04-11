using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;

namespace NewCrmCore.Application.Services.Interface
{
	public interface IWallpaperServices
	{
		#region have return value

		/// <summary>
		/// 获取所有的系统壁纸
		/// </summary>
		Task<List<WallpaperDto>> GetWallpapersAsync();

		/// <summary>
		/// 添加壁纸
		/// </summary>
		Task<Tuple<Int32, String>> AddWallpaperAsync(WallpaperDto wallpaperDto);

		/// <summary>
		/// 根据用户id获取上传的壁纸
		/// </summary>
		Task<List<WallpaperDto>> GetUploadWallpaperAsync(Int32 accountId);

		/// <summary>
		/// 根据用户id添加来自于网络的壁纸
		/// </summary>
		Task<Tuple<Int32, String>> AddWebWallpaperAsync(Int32 accountId, String url);

		/// <summary>
		/// 根据md5获取上传的壁纸
		/// </summary>
		Task<WallpaperDto> GetUploadWallpaperAsync(String md5);

		#endregion

		#region not have return value

		/// <summary>
		/// 根据用户id删除上传的壁纸
		/// </summary>
		Task RemoveWallpaperAsync(Int32 accountId, Int32 wallpaperId);

		/// <summary>
		/// 修改壁纸的显示模式
		/// </summary>
		Task ModifyWallpaperModeAsync(Int32 accountId, String newMode);

		/// <summary>
		/// 修改壁纸
		/// </summary>
		Task ModifyWallpaperAsync(Int32 accountId, Int32 newWallpaperId);

		#endregion
	}
}
