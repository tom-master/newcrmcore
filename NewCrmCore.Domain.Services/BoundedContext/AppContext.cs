﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class AppContext : IAppContext
    {
        public async Task<Tuple<Int32, Int32>> GetDevelopAndNotReleaseCountAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.Id FROM App AS a WHERE a.UserId=@userId AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@userId",userId)
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
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.Id,a.Name,a.IsSystem FROM AppType AS a WHERE a.IsDeleted=0";
                    return dataStore.Find<AppType>(sql);
                }
            });
        }

        public async Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT 
                            a.UseCount,
                            a.Id,
                            a.Name,
                            a.IconUrl AS AppIcon,
                            a.Remark,
                            a.AppStyle AS Style,
                            (
		                        SELECT AVG(stars.StartNum) FROM AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
                            ) AS AppStars,
                            (
	                            CASE 
								WHEN a2.Id IS NULL THEN FALSE
								ELSE TRUE
								END
                            ) AS IsInstall,
                            IFNULL(a.IsIconByUpload,0) AS IsIconByUpload
                            FROM App AS a 
							LEFT JOIN Member AS a2 ON a2.UserId=@userId AND a2.IsDeleted=0 AND a2.AppId=a.Id
                            WHERE a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState AND a.IsRecommand=1";

                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@AppAuditState", AppAuditState.Pass.ToInt32()),
                        new ParameterMapper("@AppReleaseState", AppReleaseState.Release.ToInt32()),
                        new ParameterMapper("@userId",userId)
                    };
                    return dataStore.FindOne<TodayRecommendAppDto>(sql, parameters);
                }
            });
        }

        public List<App> GetApps(Int32 userId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userId, true);
            Parameter.Validate(orderId);
            Parameter.Validate(pageIndex, true);
            Parameter.Validate(pageSize);

            using (var dataStore = new DataStore(Appsetting.Database))
            {
                var parameters = new List<ParameterMapper>
                {
                    new ParameterMapper("@AppAuditState", AppAuditState.Pass.ToInt32()),
                    new ParameterMapper("@AppReleaseState", AppReleaseState.Release.ToInt32())
                };

                var where = new StringBuilder();
                where.Append($@" WHERE 1=1  AND a.IsDeleted=0 AND a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState");
                if (appTypeId != 0 && appTypeId != -1)//全部app
                {
                    parameters.Add(new ParameterMapper("@AppTypeId", appTypeId));
                    where.Append($@" AND a.AppTypeId=@AppTypeId");
                }
                else
                {
                    if (appTypeId == -1)//用户制作的app
                    {
                        parameters.Add(new ParameterMapper("@userId", userId));
                        where.Append($@" AND a.UserId=@userId");
                    }
                }
                if (!String.IsNullOrEmpty(searchText))//关键字搜索
                {
                    parameters.Add(new ParameterMapper("@Name", $@"%{searchText}%"));
                    where.Append($@" AND a.Name LIKE @Name");
                }

                var orderBy = new StringBuilder();
                switch (orderId)
                {
                    case 1:
                        {
                            orderBy.Append($@" ORDER BY a.AddTime DESC");
                            break;
                        }
                    case 2:
                        {
                            orderBy.Append($@" ORDER BY a.UseCount DESC");
                            break;
                        }
                    case 3:
                        {
                            orderBy.Append($@" ORDER BY (SELECT AVG(stars.StartNum) FROM AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId) DESC");
                            break;
                        }
                }

                var paging = new PageList<App>();
                #region totalCount
                {
                    var sql = $@"SELECT COUNT(*) FROM App AS a LEFT JOIN AppStar AS a1 ON a1.AppId=a.Id AND a1.IsDeleted=0 {where}";
                    totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
                }
                #endregion

                #region sql
                {
                    var sql = $@"SELECT 
	                            a.AppTypeId,
	                            a.UserId,
	                            a.AddTime,
	                            a.UseCount,
	                            (
		                            SELECT TRUNCATE(AVG(stars.StartNum),1) FROM AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
	                            ) AS StarCount,
	                            a.Name,
	                            a.IconUrl,
	                            a.Remark,
	                            a.AppStyle,
	                            a.Id,
	                            (
		                            CASE 
			                            WHEN a1.Id IS NOT NULL THEN TRUE
			                            ELSE FALSE
		                            END
	                            ) AS IsInstall,
                                a.IsIconByUpload
	                            FROM App AS a
	                            LEFT JOIN Member AS a1 ON a1.UserId=a.UserId AND a1.AppId=a.Id AND a1.IsDeleted=0
                                {where} {orderBy} LIMIT {pageSize * (pageIndex - 1)},{pageSize }";
                    return dataStore.Find<App>(sql, parameters);
                }
                #endregion
            }
        }

        public List<App> GetUserApps(Int32 userId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userId, true);
            Parameter.Validate(appTypeId, true);
            Parameter.Validate(appStyleId, true);
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);
            using (var dataStore = new DataStore(Appsetting.Database))
            {
                var where = new StringBuilder();
                where.Append($@" WHERE 1=1 AND a.IsDeleted=0 ");
                var parameters = new List<ParameterMapper>();

                #region 条件筛选

                if (userId != default(Int32))
                {
                    parameters.Add(new ParameterMapper("@userId", userId));
                    where.Append($@" AND a.UserId=@userId");
                }

                //应用名称
                if (!String.IsNullOrEmpty(searchText))
                {
                    parameters.Add(new ParameterMapper("@Name", $@"%{searchText}%"));
                    where.Append($@" AND a.Name LIKE @Name");
                }

                //应用所属类型
                if (appTypeId != 0)
                {
                    parameters.Add(new ParameterMapper("AppTypeId", appTypeId));
                    where.Append($@" AND a.AppTypeId=@AppTypeId");
                }

                //应用样式
                if (appStyleId != 0)
                {
                    var appStyle = EnumExtensions.ToEnum<AppStyle>(appStyleId);
                    parameters.Add(new ParameterMapper("@AppStyle", appStyle.ToInt32()));
                    where.Append($@" AND a.AppStyle=@AppStyle");
                }

                if ((appState + "").Length > 0)
                {
                    //app发布状态
                    var stats = appState.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (stats[0] == "AppReleaseState")
                    {
                        var appReleaseState = EnumExtensions.ToEnum<AppReleaseState>(Int32.Parse(stats[1]));
                        parameters.Add(new ParameterMapper("AppReleaseState", appReleaseState.ToInt32()));
                        where.Append($@" AND a.AppReleaseState=@AppReleaseState ");
                    }

                    //app应用审核状态
                    if (stats[0] == "AppAuditState")
                    {
                        var appAuditState = EnumExtensions.ToEnum<AppAuditState>(Int32.Parse(stats[1]));
                        parameters.Add(new ParameterMapper("@AppAuditState", appAuditState.ToInt32()));
                        where.Append($@" AND a.AppAuditState=@AppAuditState");
                    }
                }

                #endregion

                #region totalCount
                {
                    var sql = $@"SELECT COUNT(*) FROM App AS a {where} ";
                    totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
                }
                #endregion

                #region sql
                {
                    var sql = $@"SELECT
								a.Name,
								a.AppStyle,
								a.UseCount,
								a.Id,
								a.IconUrl,
								a.AppAuditState,
								a.IsRecommand,
								a.AppTypeId,
								a.UserId,
								a.IsIconByUpload
								FROM App AS a {where} LIMIT {pageSize * (pageIndex - 1)},{pageSize}";

                    return dataStore.Find<App>(sql, parameters);
                }
                #endregion

            }
        }

        public async Task<App> GetAppAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT 
                            a.Name,
                            a.IconUrl,
                            a.Remark,
                            a.UseCount,
                            (
		                      SELECT AVG(stars.StartNum) FROM AppStar AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
	                        ) AS StarCount,
                            a.AddTime,
                            a.UserId,
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
                            a2.Name AS UserName,
                            a.IsIconByUpload
                            FROM App AS a 
                            LEFT JOIN User AS a2
                            ON a2.Id=a.UserId AND a2.IsDeleted=0 AND a2.IsDisable=0
                            WHERE a.Id=@Id AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@Id",appId)
                    };
                    return dataStore.FindOne<App>(sql, parameters);
                }
            });
        }

        public async Task<Boolean> IsInstallAppAsync(Int32 userId, Int32 appId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(appId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM Member AS a WHERE a.AppId=@Id AND a.UserId=@UserId AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@Id",appId),
                        new ParameterMapper("@UserId",userId)
                    };
                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0 ? true : false;
                }
            });
        }

        public async Task<List<App>> GetSystemAppAsync(IEnumerable<Int32> appIds = default(IEnumerable<Int32>))
        {
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var where = new StringBuilder();
                    if (appIds != default(IEnumerable<Int32>) && appIds.Any())
                    {
                        where.Append($@" AND a.Id IN({String.Join(",", appIds)})");
                    }

                    var sql = $@"SELECT a.Id,a.Name,a.IconUrl FROM App AS a WHERE a.IsSystem=1 AND a.IsDeleted=0 {where}";
                    return dataStore.Find<App>(sql);
                }
            });
        }

        public async Task<Boolean> CheckAppTypeNameAsync(String appTypeName)
        {
            Parameter.Validate(appTypeName);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM AppType AS a WHERE a.Name=@name AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@name",appTypeName)
                    };
                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }

        public async Task ModifyAppStarAsync(Int32 userId, Int32 appId, Int32 starCount)
        {
            Parameter.Validate(userId);
            Parameter.Validate(appId);
            Parameter.Validate(starCount);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 前置条件判断
                    {
                        var sql = $@"SELECT COUNT(*) FROM AppStar AS a WHERE a.UserId=@userId AND a.AppId=@appId AND a.IsDeleted=0";
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@userId",userId),
                            new ParameterMapper("@appId",appId)
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
                        var appStar = new AppStar(userId, appId, starCount);
                        dataStore.Add(appStar);
                    }
                    #endregion
                }
            });
        }

        public async Task CreateNewAppAsync(App app)
        {
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region app
                    {
                        dataStore.Add(app);
                    }
                    #endregion
                }
            });
        }

        public async Task<App> PassAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            return await Task.Run<App>(async () =>
             {
                 using (var dataStore = new DataStore(Appsetting.Database))
                 {
                     var app = await GetAppAsync(appId);
                     app.Pass();
                     var result = dataStore.Modify(app, a => a.Id == appId);
                     if (!result)
                     {
                         throw new BusinessException("应用审核状态更新失败");
                     }
                     return app;
                 }
             });
        }

        public async Task<App> DenyAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            return await Task.Run<App>(async () =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var app = await GetAppAsync(appId);
                    app.Deny();
                    var result = dataStore.Modify(app, a => a.Id == appId);
                    if (!result)
                    {
                        throw new BusinessException("应用审核状态更新失败");
                    }
                    return app;
                }
            });
        }

        public async Task SetTodayRecommandAppAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        #region 取消之前的推荐应用
                        {
                            var app = new App().CancelRecommand();
                            var result = dataStore.Modify(app, a => a.IsRecommand);
                            if (!result)
                            {
                                throw new BusinessException("取消之前的推荐应用失败");
                            }
                        }
                        #endregion

                        #region 设置新的推荐应用
                        {
                            var app = new App().Recommand();
                            var result = dataStore.Modify(app, a => a.Id == appId);
                            if (!result)
                            {
                                throw new BusinessException("设置新的推荐应用失败");
                            }
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
            Parameter.Validate(appId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        #region 移除应用的评分
                        {
                            var appStar = new AppStar();
                            appStar.Remove();
                            var result = dataStore.Modify(appStar, star => star.AppId == appId);
                            if (!result)
                            {
                                throw new BusinessException("移除应用的评分失败");
                            }
                        }
                        #endregion

                        #region 移除应用
                        {
                            var app = new App();
                            app.Remove();
                            var result = dataStore.Modify(app, a => a.Id == appId);
                            if (!result)
                            {
                                throw new BusinessException("移除应用失败");
                            }
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
            Parameter.Validate(appId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 发布应用
                    {
                        var app = new App().AppRelease().Pass();
                        var result = dataStore.Modify(app, a => a.Id == appId);
                        if (!result)
                        {
                            throw new BusinessException("发布应用失败");
                        }
                    }
                    #endregion
                }
            });
        }

        public async Task ModifyUserAppInfoAsync(Int32 userId, App app)
        {
            Parameter.Validate(userId);
            Parameter.Validate(userId);
            Parameter.Validate(app);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
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
                    var result = dataStore.Modify(app, a => a.UserId == userId && a.Id == app.Id);
                    if (!result)
                    {
                        throw new BusinessException("修改应用信息失败");
                    }
                }
            });
        }

        public async Task DeleteAppTypeAsync(Int32 appTypeId)
        {
            Parameter.Validate(appTypeId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 前置条件验证
                    {
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@AppTypeId",appTypeId)
                        };
                        var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.AppTypeId=@AppTypeId AND a.IsDeleted=0";
                        if (dataStore.FindSingleValue<Int32>(sql, parameters) > 0)
                        {
                            throw new BusinessException($@"当前分类下存在应用,不能删除当前分类");
                        }
                    }
                    #endregion

                    #region 移除应用分类
                    {
                        var appType = new AppType();
                        appType.Remove();
                        var result = dataStore.Modify(appType, type => type.Id == appTypeId);
                        if (!result)
                        {
                            throw new BusinessException("移除应用分类失败");
                        }
                    }
                    #endregion
                }
            });
        }

        public async Task CreateNewAppTypeAsync(AppType appType)
        {
            Parameter.Validate(appType);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 前置条件验证
                    {
                        var sql = $@"SELECT COUNT(*) FROM AppType AS a WHERE a.Name=@name AND a.IsDeleted=0";
                        var result = dataStore.FindSingleValue<Int32>(sql, new List<ParameterMapper> { new ParameterMapper("@name", appType.Name) });
                        if (result > 0)
                        {
                            throw new BusinessException($@"分类:{appType.Name},已存在");
                        }
                    }
                    #endregion

                    #region 添加应用分类
                    {
                        var result = dataStore.Add(appType);
                        if (result <= 0)
                        {
                            throw new BusinessException("添加应用分类失败");
                        }
                    }
                    #endregion
                }
            });
        }

        public async Task ModifyAppTypeAsync(String appTypeName, Boolean isSystem, Int32 appTypeId)
        {
            Parameter.Validate(appTypeName);
            Parameter.Validate(appTypeId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 更新应用分类
                    {
                        var appType = new AppType();
                        appType.ModifyName(appTypeName);
                        if (isSystem)
                        {
                            appType.System();
                        }
                        else
                        {
                            appType.NotSystem();
                        }
                        var result = dataStore.Modify(appType, type => type.Id == appTypeId);
                        if (!result)
                        {
                            throw new BusinessException("更新应用分类");
                        }
                    }
                    #endregion
                }
            });
        }

        public async Task ModifyAppIconAsync(Int32 userId, Int32 appId, String newIcon)
        {
            Parameter.Validate(userId);
            Parameter.Validate(appId);
            Parameter.Validate(newIcon);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var app = new App();
                    app.ModifyIconUrl(newIcon);
                    var result = dataStore.Modify(app, a => a.Id == appId && a.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改应用图标失败");
                    }
                }
            });
        }

        public async Task<App> InstallAsync(Int32 userId, Int32 appId, Int32 deskNum)
        {
            Parameter.Validate(userId);
            Parameter.Validate(appId);
            Parameter.Validate(deskNum);
            return await Task.Run<App>(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
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
                                a.IsSetbar,
                                a.IsOpenMax,
                                a.IsFlash,
                                a.IsIconByUpload,
                                a.AppStyle
                                FROM  App AS a WHERE a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState AND a.IsDeleted=0 AND a.Id=@Id";
                            var parameters = new List<ParameterMapper>
                            {
                                new ParameterMapper("@AppAuditState",AppAuditState.Pass.ToInt32()),
                                new ParameterMapper("@AppReleaseState",AppReleaseState.Release.ToInt32()),
                                new ParameterMapper("@Id",appId)
                            };
                            app = dataStore.FindOne<App>(sql, parameters);

                            if (app == null)
                            {
                                throw new BusinessException($"获取应用失败，请刷新重试");
                            }
                        }
                        #endregion

                        #region 添加桌面应用
                        {
                            var newMember = new Member(app.Name, app.IconUrl, app.AppUrl, app.Id, app.Width, app.Height, userId, deskNum, app.AppStyle, app.IsIconByUpload, app.IsSetbar, app.IsOpenMax, app.IsFlash, app.IsResize);
                            dataStore.Add(newMember);
                        }
                        #endregion

                        #region 更改应用使用数量
                        {
                            app.IncreaseUseCount();
                            var result = dataStore.Modify(app, a => a.Id == appId);
                            if (!result)
                            {
                                throw new BusinessException("修改应用使用数量失败");
                            }
                        }
                        #endregion

                        dataStore.Commit();
                        return app;
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
