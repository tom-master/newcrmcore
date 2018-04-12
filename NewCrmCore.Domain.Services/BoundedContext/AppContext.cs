using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Infrastructure.CommonTools.CustomException;
using NewLibCore;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCRM.Domain.Services.BoundedContext
{
	public class AppContext: IAppContext
	{
		public async Task<Tuple<Int32, Int32>> GetAccountDevelopAppCountAndNotReleaseAppCountAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT a.Id FROM dbo.App AS a WHERE a.AccountId=@accountId AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@accountId",accountId)
					};
					var result = dataStore.Find<App>(sql, parameters);
					return new Tuple<Int32, Int32>(result.Count, result.Count(a => a.AppReleaseState == AppReleaseState.UnRelease));
				}
			});
		}

		public async Task<List<AppType>> GetAppTypesAsync()
		{
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT a.Id,a.Name FROM dbo.AppType AS a WHERE a.IsDeleted=0";
					return dataStore.Find<AppType>(sql);
				}
			});
		}

		public async Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT 
                            a.UseCount,
                            a.Id,
                            a.Name,
                            a.IconUrl AS AppIcon,
                            a.Remark,
                            a.AppStyle AS Style,
                            (
		                        SELECT AVG(stars.StartNum) FROM dbo.AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
                            ) AS AppStars,
                            (
	                            CASE 
								WHEN a2.Id IS NULL THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
								END
                            ) AS IsInstall,
                            ISNULL(a.IsIconByUpload,0) AS IsIconByUpload
                            FROM dbo.App AS a 
							LEFT JOIN dbo.Member AS a2 ON a2.AccountId=@accountId AND a2.IsDeleted=0 AND a2.AppId=a.Id
                            WHERE a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState AND a.IsRecommand=1";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@AppAuditState",(Int32)AppAuditState.Pass),
						new SqlParameter("@AppReleaseState",(Int32)AppReleaseState.Release),
						new SqlParameter("@accountId",accountId)
					};
					return dataStore.FindOne<TodayRecommendAppDto>(sql, parameters);
				}
			});
		}

		public async Task<PagingModel<App>> GetAppsAsync(Int32 accountId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize)
		{
			new Parameter().Validate(accountId, true).Validate(orderId).Validate(searchText).Validate(pageIndex, true).Validate(pageSize);

			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@AppAuditState", (Int32)AppAuditState.Pass),
						new SqlParameter("@AppReleaseState", (Int32)AppReleaseState.Release)
					};

					var where = new StringBuilder();
					where.Append($@" WHERE 1=1 AND a.IsDeleted=0 AND a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState");
					if (appTypeId != 0 && appTypeId != -1)//全部app
					{
						parameters.Add(new SqlParameter("@AppTypeId", appTypeId));
						where.Append($@" AND a.AppTypeId=@AppTypeId");
					}
					else
					{
						if (appTypeId == -1)//用户制作的app
						{
							parameters.Add(new SqlParameter("@accountId", accountId));
							where.Append($@" AND a.AccountId=@accountId");
						}
					}
					if (!String.IsNullOrEmpty(searchText))//关键字搜索
					{
						parameters.Add(new SqlParameter("@Name", $@"%{searchText}%"));
						where.Append($@" AND a.Name LIKE @Name");
					}

					var orderBy = new StringBuilder();
					switch (orderId)
					{
						case 1:
						{
							orderBy.Append($@" ORDER BY aa.AddTime DESC");
							break;
						}
						case 2:
						{
							orderBy.Append($@" ORDER BY aa.UseCount DESC");
							break;
						}
						case 3:
						{
							orderBy.Append($@" ORDER BY aa.StarCount DESC");
							break;
						}
					}

					var paging = new PagingModel<App>();
					#region totalCount
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.App AS a 
                                LEFT JOIN dbo.AppStar AS a1
                                ON a1.AppId=a.Id AND a1.IsDeleted=0 {where}";
						paging.TotalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
					}
					#endregion

					#region sql
					{
						var sql = $@"SELECT TOP (@pageSize) * FROM 
                                (
	                                SELECT 
	                                    ROW_NUMBER() OVER(ORDER BY a.AddTime DESC) AS rownumber,
	                                    a.AppTypeId,
	                                    a.AccountId,
	                                    a.AddTime,
	                                    a.UseCount,
	                                    (
		                                    SELECT AVG(stars.StartNum) FROM dbo.AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
	                                    ) AS StarCount,
	                                    a.Name,
	                                    a.IconUrl,
	                                    a.Remark,
	                                    a.AppStyle,
	                                    a.Id,
	                                    (
		                                    CASE 
			                                    WHEN a1.Id IS NOT NULL THEN CAST(1 AS BIT)
			                                    ELSE CAST(0 AS BIT)
		                                    END
	                                    ) AS IsInstall,
                                        a.IsIconByUpload
	                                    FROM dbo.App AS a
	                                    LEFT JOIN dbo.Member AS a1 ON a1.AccountId=a.AccountId AND a1.AppId=a.Id AND a1.IsDeleted=0
                                        {where}
                                ) AS aa WHERE aa.rownumber>@pageSize*(@pageIndex-1) {orderBy}";
						parameters.Add(new SqlParameter("@pageSize", pageSize));
						parameters.Add(new SqlParameter("@pageIndex", pageIndex));
						paging.Models = dataStore.Find<App>(sql, parameters);
					}
					#endregion

					return paging;
				}
			});
		}

		public async Task<PagingModel<App>> GetAccountAppsAsync(Int32 accountId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize)
		{
			new Parameter().Validate(accountId, true).Validate(searchText).Validate(appTypeId, true).Validate(appStyleId, true).Validate(pageIndex).Validate(pageSize);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var where = new StringBuilder();
					where.Append($@" WHERE 1=1 AND a.IsDeleted=0 ");
					var parameters = new List<SqlParameter>();

					#region 条件筛选

					if (accountId != default(Int32))
					{
						parameters.Add(new SqlParameter("@accountId", accountId));
						where.Append($@" AND a.AccountId=@accountId");
					}

					//应用名称
					if (!String.IsNullOrEmpty(searchText))
					{
						parameters.Add(new SqlParameter("@Name", $@"%{searchText}%"));
						where.Append($@" AND a.Name LIKE @Name");
					}

					//应用所属类型
					if (appTypeId != 0)
					{
						parameters.Add(new SqlParameter("AppTypeId", appTypeId));
						where.Append($@" AND a.AppTypeId=@AppTypeId");
					}

					//应用样式
					if (appStyleId != 0)
					{
						var appStyle = EnumExtensions.ToEnum<AppStyle>(appStyleId);
						parameters.Add(new SqlParameter("@AppStyle", (Int32)appStyle));
						where.Append($@" AND a.AppStyle=@AppStyle");
					}

					if ((appState + "").Length > 0)
					{
						//app发布状态
						var stats = appState.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						if (stats[0] == "AppReleaseState")
						{
							var appReleaseState = EnumExtensions.ToEnum<AppReleaseState>(Int32.Parse(stats[1]));
							parameters.Add(new SqlParameter("AppReleaseState", (Int32)appReleaseState));
							where.Append($@" AND a.AppReleaseState=@AppReleaseState ");
						}

						//app应用审核状态
						if (stats[0] == "AppAuditState")
						{
							var appAuditState = EnumExtensions.ToEnum<AppAuditState>(Int32.Parse(stats[1]));
							parameters.Add(new SqlParameter("@AppAuditState", (Int32)appAuditState));
							where.Append($@" AND a.AppAuditState=@AppAuditState");
						}
					}

					#endregion
					var paging = new PagingModel<App>();
					#region totalCount
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.App AS a {where} ";
						paging.TotalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
					}
					#endregion

					#region sql
					{
						var sql = $@"SELECT TOP (@pageSize) * FROM 
								(
									SELECT
									ROW_NUMBER() OVER(ORDER BY a.Id DESC) AS rownumber,
									a.Name,
									a.AppStyle,
									a.UseCount,
									a.Id,
									a.IconUrl,
									a.AppAuditState,
									a.IsRecommand,
									a.AppTypeId,
									a.AccountId,
									a.IsIconByUpload
									FROM dbo.App AS a {where} 
								) AS aa WHERE aa.rownumber>@pageSize*(@pageIndex-1)";
						parameters.Add(new SqlParameter("@pageIndex", pageIndex));
						parameters.Add(new SqlParameter("@pageSize", pageSize));
						paging.Models = dataStore.Find<App>(sql, parameters);
					}
					#endregion

					return paging;
				}
			});
		}

		public async Task<App> GetAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT 
                            a.Name,
                            a.IconUrl,
                            a.Remark,
                            a.UseCount,
                            (
		                      SELECT AVG(stars.StartNum) FROM dbo.AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
	                        ) AS StarCount,
                            a.AddTime,
                            a.AccountId,
                            a.Id,
                            a.IsResize,
                            a.IsOpenMax,
                            a.IsFlash,
                            a.AppStyle,
                            a.AppUrl,
                            a.Width,
                            a.Height,
                            a.AppAuditState,
                            a.AppReleaseState,
                            a.AppTypeId,
                            a2.Name AS AccountName,
                            a.IsIconByUpload
                            FROM dbo.App AS a 
                            LEFT JOIN dbo.Account AS a2
                            ON a2.Id=a.AccountId AND a2.IsDeleted=0 AND a2.IsDisable=0
                            WHERE a.Id=@Id AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@Id",appId)
					};
					return dataStore.FindOne<App>(sql, parameters);
				}
			});
		}

		public async Task<Boolean> IsInstallAppAsync(Int32 accountId, Int32 appId)
		{
			new Parameter().Validate(accountId).Validate(appId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT COUNT(*) FROM dbo.Member AS a WHERE a.AppId=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@Id",appId),
						new SqlParameter("@AccountId",accountId)
					};
					return dataStore.FindSingleValue<Int32>(sql, parameters) > 0 ? true : false;
				}
			});
		}

		public async Task<List<App>> GetSystemAppAsync(IEnumerable<Int32> appIds = default(IEnumerable<Int32>))
		{
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var where = new StringBuilder();
					where.Append(" WHERE 1=1 AND a.IsSystem=1 AND a.IsDeleted=0");
					if (appIds != default(IEnumerable<Int32>) && appIds.Any())
					{
						where.Append($@" AND a.Id IN({String.Join(",", appIds)})");
					}

					var sql = $@"SELECT a.Id,a.Name,a.IconUrl FROM dbo.App AS a {where}";
					return dataStore.Find<App>(sql);
				}
			});
		}

		public async Task<Boolean> CheckAppTypeNameAsync(String appTypeName)
		{
			new Parameter().Validate(appTypeName);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT COUNT(*) FROM dbo.AppType AS a WHERE a.Name=@name AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@name",appTypeName)
					};
					return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
				}
			});
		}

		public async Task ModifyAppStarAsync(Int32 accountId, Int32 appId, Int32 starCount)
		{
			new Parameter().Validate(accountId).Validate(appId).Validate(starCount);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 前置条件判断
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.AppStar AS a WHERE a.AccountId=@accountId AND a.AppId=@appId AND a.IsDeleted=0";
						var parameters = new List<SqlParameter>
						{
							new SqlParameter("@accountId",accountId),
							new SqlParameter("@appId",appId)
						};
						var result = dataStore.FindSingleValue<Int32>(sql, parameters);
						if (result > 0)
						{
							throw new BusinessException("您已为这个应用打分");
						}
					}
					#endregion

					#region sql
					{
						var appStar = new AppStar(accountId, appId, starCount);
						dataStore.ExecuteAdd(appStar);
					}
					#endregion
				}
			});
		}

		public async Task CreateNewAppAsync(App app)
		{
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region app
					{
						dataStore.ExecuteAdd(app);
					}
					#endregion
				}
			});
		}

		public async Task PassAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var app = new App().Pass();
					dataStore.ExecuteModify(app, a => a.Id == appId);
				}
			});
		}

		public async Task DenyAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var app = new App().Deny();
					dataStore.ExecuteModify(app, a => a.Id == appId);
				}
			});
		}

		public async Task SetTodayRecommandAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					dataStore.OpenTransaction();
					try
					{
						#region 取消之前的推荐app
						{
							var app = new App().CancelRecommand();
							dataStore.ExecuteModify(app, a => a.IsRecommand == true);
						}
						#endregion

						#region 设置新的推荐app
						{
							var app = new App().Recommand();
							dataStore.ExecuteModify(app, a => a.Id == appId);
						}
						#endregion

						dataStore.Commit();
					}
					catch (Exception)
					{
						dataStore.Rollback();
						throw;
					}
				}
			});
		}

		public async Task RemoveAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					dataStore.OpenTransaction();
					try
					{
						#region 移除app的评分
						{
							var appStar = new AppStar();
							appStar.Remove();
							dataStore.ExecuteModify(appStar, star => star.AppId == appId);
						}
						#endregion

						#region 移除app
						{
							var app = new App();
							app.Remove();
							dataStore.ExecuteModify(app, a => a.Id == appId);
						}
						#endregion

						dataStore.Commit();
					}
					catch (Exception)
					{
						dataStore.Rollback();
						throw;
					}
				}
			});
		}

		public async Task ReleaseAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 发布app
					{
						var app = new App().AppRelease().Pass();
						dataStore.ExecuteModify(app, a => a.Id == appId);
					}
					#endregion
				}
			});
		}

		public async Task ModifyAccountAppInfoAsync(Int32 accountId, App app)
		{
			new Parameter().Validate(accountId).Validate(accountId).Validate(app);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					if (app.IsIconByUpload)
					{
						app.IconFromUpload();
					}
					else
					{
						app.IconNotFromUpload();
					}
					app.ModifyIconUrl(app.IconUrl);
					app.ModifyName(app.Name);
					app.ModifyAppTypeId(app.AppTypeId);
					app.ModifyUrl(app.AppUrl);
					app.ModifyWidth(app.Width);
					app.ModifyHeight(app.Height);
					if (app.AppStyle == AppStyle.App)
					{
						app.StyleToApp();
					}
					else
					{
						app.StyleToWidget();
					}

					if (app.IsResize)
					{
						app.Resize();
					}
					else
					{
						app.NotResize();
					}

					if (app.IsOpenMax)
					{
						app.OpenMax();
					}
					else
					{
						app.NotOpenMax();
					}

					if (app.IsFlash)
					{
						app.Flash();
					}
					else
					{
						app.NotFlash();
					}


					app.ModifyRemark(app.Remark);

					if (app.AppAuditState == AppAuditState.Wait)
					{
						app.Wait();
					}
					else
					{
						app.UnAuditState();
					}
					dataStore.ExecuteModify(app, a => a.AccountId == accountId && a.Id == app.Id);
				}
			});
		}

		public async Task DeleteAppTypeAsync(Int32 appTypeId)
		{
			new Parameter().Validate(appTypeId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 前置条件验证
					{
						var parameters = new List<SqlParameter>
						{
							new SqlParameter("@AppTypeId",appTypeId)
						};
						var sql = $@"SELECT COUNT(*) FROM dbo.App AS a WHERE a.AppTypeId=@AppTypeId AND a.IsDeleted=0";
						if (dataStore.FindSingleValue<Int32>(sql, parameters) > 0)
						{
							throw new BusinessException($@"当前分类下已有绑定app,不能删除当前分类");
						}
					}
					#endregion

					#region 移除app分类
					{
						var appType = new AppType();
						appType.Remove();
						dataStore.ExecuteModify(appType, type => type.Id == appTypeId);
					}
					#endregion
				}
			});
		}

		public async Task CreateNewAppTypeAsync(AppType appType)
		{
			new Parameter().Validate(appType);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 前置条件验证
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.AppType AS a WHERE a.Name=@name AND a.IsDeleted=0";
						var result = dataStore.FindSingleValue<Int32>(sql, new List<SqlParameter> { new SqlParameter("@name", appType.Name) });
						if (result > 0)
						{
							throw new BusinessException($@"分类:{appType.Name},已存在");
						}
					}
					#endregion

					#region 添加app分类
					{
						dataStore.ExecuteAdd(appType);
					}
					#endregion
				}
			});
		}

		public async Task ModifyAppTypeAsync(String appTypeName, Int32 appTypeId)
		{
			new Parameter().Validate(appTypeName).Validate(appTypeId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 前置条件验证
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.AppType AS a WHERE a.Name=@name AND a.IsDeleted=0";
						var result = dataStore.FindSingleValue<Int32>(sql, new List<SqlParameter> { new SqlParameter("@name", appTypeName) });
						if (result > 0)
						{
							throw new BusinessException($@"分类:{appTypeName},已存在");
						}
					}
					#endregion

					#region 更新app分类
					{
						var appType = new AppType();
						appType.ModifyName(appTypeName);
						dataStore.ExecuteModify(appType, type => type.Id == appTypeId);
					}
					#endregion
				}
			});
		}

		public async Task ModifyAppIconAsync(Int32 accountId, Int32 appId, String newIcon)
		{
			new Parameter().Validate(accountId).Validate(appId).Validate(newIcon);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var app = new App();
					app.ModifyIconUrl(newIcon);
					dataStore.ExecuteModify(app, a => a.Id == appId && a.AccountId == accountId);
				}
			});
		}

		public async Task InstallAsync(Int32 accountId, Int32 appId, Int32 deskNum)
		{
			new Parameter().Validate(accountId).Validate(appId).Validate(deskNum);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					dataStore.OpenTransaction();
					try
					{
						App app = null;
						#region 获取app
						{
							var sql = $@"SELECT
                                a.Name,
                                a.IconUrl,
                                a.AppUrl,
                                a.Id,
                                a.Width,
                                a.Height,
                                a.IsLock,
                                a.IsMax,
                                a.IsFull,
                                a.IsSetbar,
                                a.IsOpenMax,
                                a.IsFlash,
                                a.IsDraw,
                                a.IsIconByUpload
                                FROM  dbo.App AS a WHERE a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState AND a.IsDeleted=0 AND a.Id=@Id";
							var parameters = new List<SqlParameter>
							{
								new SqlParameter("@AppAuditState",(Int32)AppAuditState.Pass),
								new SqlParameter("@AppReleaseState",(Int32)AppReleaseState.Release),
								new SqlParameter("@Id",appId)
							};
							app = dataStore.FindOne<App>(sql, parameters);

							if (app == null)
							{
								throw new BusinessException($"获取应用失败，请刷新重试");
							}
						}
						#endregion

						#region 添加桌面成员
						{
							var newMember = new Member(app.Name, app.IconUrl, app.AppUrl, app.Id, app.Width, app.Height, accountId, deskNum, app.IsIconByUpload, app.IsLock, app.IsMax, app.IsFull, app.IsSetbar, app.IsOpenMax, app.IsFlash, app.IsDraw, app.IsResize);
							dataStore.ExecuteAdd(newMember);
						}
						#endregion

						#region 更改app使用数量
						{
							app.IncreaseUseCount();
							dataStore.ExecuteModify(app, a => a.Id == appId);
						}
						#endregion

						dataStore.Commit();
					}
					catch (Exception)
					{
						dataStore.Rollback();
						throw;
					}
				}
			});
		}
	}
}
