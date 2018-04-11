using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NewCRM.Domain.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure.CommonTools.CustomException;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCRM.Domain.Services.BoundedContext
{
	public class WallpaperContext: IWallpaperContext
	{
		public async Task<Tuple<Int32, String>> AddWallpaperAsync(Wallpaper wallpaper)
		{
			new Parameter().Validate(wallpaper);
			return await Task.Run(() =>
			 {
				 using (var dataStore = new DataStore())
				 {
					 #region 前置条件验证
					 {
						 var sql = $@"SELECT COUNT(*) FROM dbo.Wallpaper AS a WHERE a.AccountId=@AccountId AND a.IsDeleted=0";
						 var parameters = new List<SqlParameter>
						 {
							new SqlParameter("@AccountId",wallpaper.AccountId)
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
						 newWallpaperId = dataStore.ExecuteAdd(wallpaper);
					 }
					 #endregion

					 #region 获取返回值
					 {
						 var sql = $@"SELECT a.Id,a.Url FROM dbo.Wallpaper AS a WHERE a.Id=@parameters AND a.IsDeleted=0";
						 var parameters = new List<SqlParameter>
						 {
							new SqlParameter("@Id",newWallpaperId)
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
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT
                            a.AccountId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                            FROM dbo.Wallpaper AS a WHERE a.Md5=@Md5 AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@Md5",md5)
					};
					return dataStore.FindOne<Wallpaper>(sql, parameters);
				}
			});
		}

		public async Task<List<Wallpaper>> GetUploadWallpaperAsync(Int32 accountId)
		{
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT
                            a.AccountId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                            FROM dbo.Wallpaper AS a WHERE a.AccountId=@AccountId AND a.Source=@Source AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@AccountId",accountId),
						new SqlParameter("@Source",(Int32)WallpaperSource.Upload)
					};
					return dataStore.Find<Wallpaper>(sql, parameters);
				}
			});
		}

		public async Task<List<Wallpaper>> GetWallpapersAsync()
		{
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT
                            a.AccountId,
                            a.Height,
                            a.Id,
                            a.Md5,
                            a.ShortUrl,
                            a.Source,
                            a.Title,
                            a.Url,
                            a.Width
                            FROM dbo.Wallpaper AS a WHERE a.Source=@Source AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@Source",(Int32)WallpaperSource.System)
					};
					return dataStore.Find<Wallpaper>(sql, parameters);
				}
			});
		}

		public async Task ModifyWallpaperModeAsync(Int32 accountId, String newMode)
		{
			new Parameter().Validate(accountId).Validate(newMode);
			await Task.Run(() =>
			{
				if (Enum.TryParse(newMode, true, out WallpaperMode wallpaperMode))
				{
					using (var dataStore = new DataStore())
					{
						var config = new Config();
						config.ModeTo(wallpaperMode);
						dataStore.ExecuteModify(config, conf => conf.AccountId == accountId);
					}
				}
				else
				{
					throw new BusinessException($"无法识别的壁纸显示模式:{newMode}");
				}
			});
		}

		public async Task ModifyWallpaperAsync(Int32 accountId, Int32 newWallpaperId)
		{
			new Parameter().Validate(accountId).Validate(newWallpaperId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var config = new Config();
					config.NotFromBing().ModifyWallpaperId(newWallpaperId);
					dataStore.ExecuteModify(config, conf => conf.AccountId == accountId);
				}
			});
		}

		public async Task RemoveWallpaperAsync(Int32 accountId, Int32 wallpaperId)
		{
			new Parameter().Validate(accountId).Validate(wallpaperId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@WallpaperId",wallpaperId),
						new SqlParameter("@AccountId",accountId)
					};
					#region 前置条件验证
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.Config AS a WHERE a.AccountId=@AccountId AND a.WallpaperId=@WallpaperId AND a.IsDeleted=0";
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
						dataStore.ExecuteModify(wallpaper, wa => wa.Id == wallpaperId && wa.AccountId == accountId);
					}
					#endregion
				}
			});
		}
	}
}
