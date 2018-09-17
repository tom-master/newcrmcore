using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;
using NewLibCore;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class WallpaperContext : IWallpaperContext
    {
        public async Task<Tuple<Int32, String>> AddWallpaperAsync(Wallpaper wallpaper)
        {
            Parameter.Validate(wallpaper);
            return await Task.Run(() =>
             {
                 using (var dataStore = new DataStore(Appsetting.Database))
                 {
                     #region 前置条件验证
                     {
                         var sql = $@"SELECT COUNT(*) FROM Wallpaper AS a WHERE a.UserId=@UserId AND a.IsDeleted=0";
                         var parameters = new List<ParameterMapper>
                         {
                            new ParameterMapper("@UserId",wallpaper.UserId)
                         };
                         var result = dataStore.FindSingleValue<Int32>(sql, parameters);
                         if (result > 6)
                         {
                             throw new BusinessException("最多只能上传6张图片");
                         }
                     }
                     #endregion

                     var newWallpaperId = 0;

                     #region 插入壁纸
                     {
                         newWallpaperId = dataStore.Add(wallpaper);
                     }
                     #endregion

                     #region 获取返回值
                     {
                         var sql = $@"SELECT a.Id,a.Url FROM Wallpaper AS a WHERE a.Id=@Id AND a.IsDeleted=0";
                         var parameters = new List<ParameterMapper>
                         {
                            new ParameterMapper("@Id",newWallpaperId)
                         };
                         var result = dataStore.FindOne<Wallpaper>(sql, parameters);
                         if (result != null)
                         {
                             return new Tuple<Int32, String>(result.Id, result.Url);
                         }
                         return null;
                     }
                     #endregion
                 }
             });
        }

        public async Task<Wallpaper> GetUploadWallpaperAsync(String md5)
        {
            Parameter.Validate(md5);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT
                            a.UserId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                            FROM Wallpaper AS a WHERE a.Md5=@Md5 AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@Md5",md5)
                    };
                    return dataStore.FindOne<Wallpaper>(sql, parameters);
                }
            });
        }

        public async Task<List<Wallpaper>> GetUploadWallpaperAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT
                            a.UserId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                            FROM Wallpaper AS a WHERE a.UserId=@UserId AND a.Source<>@Source AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@UserId",userId),
                        new ParameterMapper("@Source", WallpaperSource.System.ToInt32())
                    };
                    return dataStore.Find<Wallpaper>(sql, parameters);
                }
            });
        }

        public async Task<List<Wallpaper>> GetWallpapersAsync()
        {
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT
                            a.UserId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                            FROM Wallpaper AS a WHERE a.Source=@Source AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@Source", WallpaperSource.System.ToInt32())
                    };
                    return dataStore.Find<Wallpaper>(sql, parameters);
                }
            });
        }

        public async Task ModifyWallpaperModeAsync(Int32 userId, String newMode)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newMode);

            await Task.Run(() =>
            {
                if (Enum.TryParse(newMode, true, out WallpaperMode wallpaperMode))
                {
                    using (var dataStore = new DataStore(Appsetting.Database))
                    {
                        var config = new Config();
                        config.ModeTo(wallpaperMode);
                        var result = dataStore.Modify(config, conf => conf.UserId == userId);
                        if (!result)
                        {
                            throw new BusinessException("修改壁纸显示失败");
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
            Parameter.Validate(userId);
            Parameter.Validate(newWallpaperId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.NotFromBing().ModifyWallpaperId(newWallpaperId);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改壁纸失败");
                    }
                }
            });
        }

        public async Task RemoveWallpaperAsync(Int32 userId, Int32 wallpaperId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(wallpaperId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 前置条件验证
                    {
                        var sql = $@"SELECT COUNT(*) FROM Config AS a WHERE a.UserId=@UserId AND a.WallpaperId=@WallpaperId AND a.IsDeleted=0";
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@WallpaperId",wallpaperId),
                            new ParameterMapper("@UserId",userId)
                        };
                        var result = dataStore.FindSingleValue<Int32>(sql, parameters);
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
                        var result = dataStore.Modify(wallpaper, wa => wa.Id == wallpaperId && wa.UserId == userId);
                        if (!result)
                        {
                            throw new BusinessException("移除壁纸失败");
                        }
                    }
                    #endregion
                }
            });
        }
    }
}
