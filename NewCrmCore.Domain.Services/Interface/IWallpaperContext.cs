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
        /// <returns></returns>
        Task<List<Wallpaper>> GetWallpapersAsync();

        /// <summary>
        /// 添加壁纸
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <returns></returns>
        Task<(Int32 wapperId, String url)> AddWallpaperAsync(Wallpaper wallpaper);

        /// <summary>
        /// 获取上传的壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Wallpaper>> GetUploadWallpaperAsync(Int32 userId);

        /// <summary>
        /// 获取上传的壁纸
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        Task<Wallpaper> GetUploadWallpaperAsync(String md5);

        /// <summary>
        /// 修改壁纸的显示模式
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newMode"></param>
        /// <returns></returns>
        Task ModifyWallpaperModeAsync(Int32 userId, String newMode);

        /// <summary>
        /// 更换壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newWallpaperId"></param>
        /// <returns></returns>
        Task ModifyWallpaperAsync(Int32 userId, Int32 newWallpaperId);

        /// <summary>
        /// 根据用户id删除壁纸
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wallpaperId"></param>
        /// <returns></returns>
        Task RemoveWallpaperAsync(Int32 userId, Int32 wallpaperId);
    }
}
