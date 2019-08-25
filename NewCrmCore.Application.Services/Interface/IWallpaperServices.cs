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
        /// <returns></returns>
        Task<List<WallpaperDto>> GetWallpapersAsync();

        /// <summary>
        /// 添加壁纸
        /// </summary>
        /// <param name="wallpaperDto"></param>
        /// <returns></returns>
       Task<(Int32 wapperId, String url)> AddWallpaperAsync(WallpaperDto wallpaperDto);

        /// <summary>
        /// 根据用户id获取上传的壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<WallpaperDto>> GetUploadWallpaperAsync(Int32 userId);

        /// <summary>
        /// 根据用户id添加来自于网络的壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<(Int32 wapperId, String url)> AddWebWallpaperAsync(Int32 userId, String url);

        /// <summary>
        /// 根据md5获取上传的壁纸
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        Task<WallpaperDto> GetUploadWallpaperAsync(String md5);

        #endregion

        #region not have return value

        /// <summary>
        /// 根据用户id删除上传的壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wallpaperId"></param>
        /// <returns></returns>
        Task RemoveWallpaperAsync(Int32 userId, Int32 wallpaperId);

        /// <summary>
        /// 修改壁纸的显示模式
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newMode"></param>
        /// <returns></returns>
        Task ModifyWallpaperModeAsync(Int32 userId, String newMode);

        /// <summary>
        /// 修改壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newWallpaperId"></param>
        /// <returns></returns>
        Task ModifyWallpaperAsync(Int32 userId, Int32 newWallpaperId);

        #endregion
    }
}
