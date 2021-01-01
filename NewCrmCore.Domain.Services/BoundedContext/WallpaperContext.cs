using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Storage.SQL;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class WallpaperContext : IWallpaperContext
    {
        public async Task<(Int32 wapperId, String url)> AddWallpaperAsync(Wallpaper wallpaper)
        {
            Check.IfNullOrZero(wallpaper);
            return await Task.Run(() =>
             {
                 using var mapper = EntityMapper.CreateMapper();
                 {
                     try
                     {
                         #region 前置条件验证
                         {
                             var result = mapper.Query<Wallpaper>().Where(w => w.UserId == wallpaper.UserId).Count();
                             if (result > 6)
                             {
                                 throw new BusinessException("最多只能上传6张图片");
                             }
                         }
                         #endregion

                         #region 插入壁纸
                         {
                             wallpaper = mapper.Add(wallpaper);
                             return (wallpaper.Id, wallpaper.Url);
                         }
                         #endregion
                     }
                     catch (System.Exception)
                     {

                         throw;
                     }
                 }
             });
        }

        public async Task<Wallpaper> GetUploadWallpaperAsync(String md5)
        {
            Check.IfNullOrZero(md5);

            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        return mapper.Query<Wallpaper>()
                        .Where(a => a.Md5 == md5)
                        .Select(a => new
                        {
                            a.UserId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                        }).FirstOrDefault();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<List<Wallpaper>> GetUploadWallpaperAsync(Int32 userId)
        {
            Check.IfNullOrZero(userId);

            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {

                        return mapper.Query<Wallpaper>()
                        .Where(a => a.UserId == userId && a.Source != WallpaperSource.System)
                        .Select(a => new
                        {
                            a.UserId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                        }).ToList();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<List<Wallpaper>> GetWallpapersAsync()
        {
            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        return mapper.Query<Wallpaper>()
                        .Where(a => a.Source == WallpaperSource.System)
                        .Select(a => new
                        {
                            a.UserId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                        }).ToList();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task ModifyWallpaperModeAsync(Int32 userId, String newMode)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(newMode);

            await Task.Run(() =>
            {
                if (Enum.TryParse(newMode, true, out WallpaperMode wallpaperMode))
                {
                    using var mapper = EntityMapper.CreateMapper();
                    {
                        try
                        {
                            var config = new Config();
                            config.ModeTo(wallpaperMode);
                            var result = mapper.Update(config, conf => conf.UserId == userId);
                            if (!result)
                            {
                                throw new BusinessException("修改壁纸显示失败");
                            }
                        }
                        catch (System.Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {
                    throw new BusinessException($"无法识别的壁纸显示模式:{newMode}");
                }
            });
        }

        public async Task ModifyWallpaperAsync(Int32 userId, Int32 newWallpaperId)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(newWallpaperId);

            await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        var config = new Config();
                        config.NotFromBing().ModifyWallpaperId(newWallpaperId);
                        var result = mapper.Update(config, conf => conf.UserId == userId);
                        if (!result)
                        {
                            throw new BusinessException("修改壁纸失败");
                        }
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
            });
        }

        public async Task RemoveWallpaperAsync(Int32 userId, Int32 wallpaperId)
        {
            Check.IfNullOrZero(userId);
            Check.IfNullOrZero(wallpaperId);

            await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                try
                {
                    #region 前置条件验证
                    {
                        var result = mapper.Query<Config>().Where(a => a.UserId == userId && a.WallpaperId == wallpaperId).Count();
                        if (result > 0)
                        {
                            throw new BusinessException("当前壁纸正在使用中，不能删除");
                        }
                    }
                    #endregion

                    #region 移除壁纸
                    {
                        var wallpaper = new Wallpaper();
                        wallpaper.Remove();
                        var result = mapper.Update(wallpaper, wa => wa.Id == wallpaperId && wa.UserId == userId);
                        if (!result)
                        {
                            throw new BusinessException("移除壁纸失败");
                        }
                    }
                    #endregion
                }
                catch (System.Exception)
                {
                    throw;
                }
            });
        }
    }
}
