using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Data.SQL.Mapper.Filter;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class AppContext : IAppContext
    {
        public async Task<(Int32 allCount, Int32 notReleaseCount)> GetDevelopAndNotReleaseCountAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    var result = mapper.Query<App>()
                    .Where(w => w.UserId == userId)
                    .Select(a => new { a.Id, a.AppReleaseState }).ToList();
                    return (result.Count, result.Count(a => a.AppReleaseState == AppReleaseState.UnRelease));
                }
            });
        }

        public async Task<List<AppType>> GetAppTypesAsync()
        {
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Query<AppType>().Select(a => new { a.Id, a.Name, a.IsSystem }).ToList();
                }
            });
        }

        public async Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    var sql = $@"SELECT 
                            a.UseCount,
                            a.Id,
                            a.Name,
                            a.IconUrl AS AppIcon,
                            a.Remark,
                            a.AppStyle AS Style,
                            (
		                        SELECT AVG(stars.StartNum) FROM newcrm_app_star AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
                            ) AS AppStars,
                            (
	                            CASE 
								WHEN a2.Id IS NULL THEN FALSE
								ELSE TRUE
								END
                            ) AS IsInstall,
                            IFNULL(a.IsIconByUpload,0) AS IsIconByUpload
                            FROM newcrm_app AS a 
							LEFT JOIN newcrm_user_member AS a2 ON a2.UserId=@userId AND a2.IsDeleted=0 AND a2.AppId=a.Id
                            WHERE a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState AND a.IsRecommand=1";

                    var parameters = new List<MapperParameter>
                    {
                        new MapperParameter("AppAuditState", AppAuditState.Pass.ToInt32()),
                        new MapperParameter("AppReleaseState", AppReleaseState.Release.ToInt32()),
                        new MapperParameter("userId",userId)
                    };
                    return mapper.SqlQuery(sql, parameters).FirstOrDefault<TodayRecommendAppDto>();
                }
            });
        }

        public List<App> GetApps(Int32 userId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userId, true);
            Parameter.Validate(orderId);
            Parameter.Validate(pageIndex, true);
            Parameter.Validate(pageSize);

            using (var mapper = EntityMapper.CreateMapper())
            {
                var parameters = new List<MapperParameter>
                {
                    new MapperParameter("AppAuditState", AppAuditState.Pass.ToInt32()),
                    new MapperParameter("AppReleaseState", AppReleaseState.Release.ToInt32())
                };

                var where = new StringBuilder();
                where.Append($@" WHERE 1=1  AND a.IsDeleted=0 AND a.AppAuditState=@AppAuditState AND a.AppReleaseState=@AppReleaseState");
                if (appTypeId != 0 && appTypeId != -1)//全部app
                {
                    parameters.Add(new MapperParameter("AppTypeId", appTypeId));
                    where.Append($@" AND a.AppTypeId=@AppTypeId");
                }
                else
                {
                    if (appTypeId == -1)//用户制作的app
                    {
                        parameters.Add(new MapperParameter("userId", userId));
                        where.Append($@" AND a.UserId=@userId");
                    }
                }
                if (!String.IsNullOrEmpty(searchText))//关键字搜索
                {
                    parameters.Add(new MapperParameter("Name", $@"%{searchText}%"));
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
                            orderBy.Append($@" ORDER BY (SELECT AVG(stars.StartNum) FROM newcrm_app_star AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId) DESC");
                            break;
                        }
                }

                var paging = new PageList<App>();
                #region totalCount
                {
                    var sql = $@"SELECT COUNT(*) FROM newcrm_app AS a LEFT JOIN newcrm_app_star AS a1 ON a1.AppId=a.Id AND a1.IsDeleted=0 {where}";
                    totalCount = mapper.SqlQuery(sql, parameters).FirstOrDefault<Int32>();
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
		                            SELECT TRUNCATE(AVG(stars.StartNum),1) FROM newcrm_app_star AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
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
	                            FROM newcrm_app AS a
	                            LEFT JOIN newcrm_user_member AS a1 ON a1.UserId=@userId2 AND a1.AppId=a.Id AND a1.IsDeleted=0
                                {where} {orderBy} LIMIT {pageSize * (pageIndex - 1)},{pageSize }";
                    parameters.Add(new MapperParameter("userId2", userId));
                    return mapper.SqlQuery(sql, parameters).ToList<App>();
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

            using (var mapper = EntityMapper.CreateMapper())
            {

                var where = FilterFactory.Create<App>();
                var parameters = new List<MapperParameter>();
                #region 条件筛选

                if (userId != default)
                {
                    where.And(w => w.UserId == userId);
                }

                //应用名称
                if (!String.IsNullOrEmpty(searchText))
                {
                    where.And(w => w.Name.Contains(searchText));
                }

                //应用所属类型
                if (appTypeId != 0)
                {
                    where.And(w => w.AppTypeId == appTypeId);
                }

                //应用样式
                if (appStyleId != 0)
                {
                    var appStyle = EnumExtensions.ToEnum<AppStyle>(appStyleId);
                    where.And(w => w.AppStyle == appStyle);
                }

                if ((appState + "").Length > 0)
                {
                    //app发布状态
                    var stats = appState.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (stats[0] == "AppReleaseState")
                    {
                        var appReleaseState = EnumExtensions.ToEnum<AppReleaseState>(Int32.Parse(stats[1]));
                        where.And(w => w.AppReleaseState == appReleaseState);
                    }

                    //app应用审核状态
                    if (stats[0] == "AppAuditState")
                    {
                        var appAuditState = EnumExtensions.ToEnum<AppAuditState>(Int32.Parse(stats[1]));
                        where.And(w => w.AppAuditState == appAuditState);
                    }
                }

                #endregion

                #region totalCount
                {
                    totalCount = mapper.Query<App>().Where(where).Count();
                }
                #endregion

                #region sql
                {
                    return mapper.Query<App>().Where(where).Select(a => new
                    {
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
                    }).Page(pageIndex, pageSize).ToList();
                }
                #endregion

            }
        }

        public async Task<App> GetAppAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    var sql = $@"SELECT 
                            a.Name,
                            a.IconUrl,
                            a.Remark,
                            a.UseCount,
                            (
		                      SELECT AVG(stars.StartNum) FROM newcrm_app_star AS stars WHERE stars.AppId=a.Id AND stars.IsDeleted=0 GROUP BY stars.AppId
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
                            FROM newcrm_app AS a 
                            LEFT JOIN newcrm_user AS a2
                            ON a2.Id=a.UserId AND a2.IsDeleted=0 AND a2.IsDisable=0
                            WHERE a.Id=@Id AND a.IsDeleted=0";
                    var parameters = new List<MapperParameter>
                    {
                        new MapperParameter("Id",appId)
                    };
                    return mapper.SqlQuery(sql, parameters).FirstOrDefault<App>();
                }
            });
        }

        public async Task<Boolean> IsInstallAppAsync(Int32 userId, Int32 appId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(appId);
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Query<Member>().Where(w => w.AppId == appId).Count() > 0;
                }
            });
        }

        public async Task<List<App>> GetSystemAppAsync(IEnumerable<Int32> appIds)
        {
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    var where = FilterFactory.Create<App>(w => w.IsSystem);
                    if (appIds != null)
                    {
                        where.And(w => appIds.Contains(w.Id));
                    }
                    return mapper.Query<App>().Select(a => new { a.Id, a.Name, a.IconUrl }).Where(where).ToList();
                }
            });
        }

        public async Task<Boolean> CheckAppTypeNameAsync(String appTypeName)
        {
            Parameter.Validate(appTypeName);
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Query<AppType>().Count() > 0;
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    #region 前置条件判断
                    {
                        var result = mapper.Query<AppStar>().Where(a => a.UserId == userId && a.AppId == appId).Count();
                        if (result > 0)
                        {
                            throw new BusinessException("您已为这个应用打分");
                        }
                    }
                    #endregion

                    #region sql
                    {
                        var appStar = new AppStar(userId, appId, starCount);
                        mapper.Add(appStar);
                    }
                    #endregion
                }
            });
        }

        public async Task CreateNewAppAsync(App app)
        {
            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.Add(app);
                }
            });
        }

        public async Task<App> PassAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            return await Task.Run<App>(async () =>
             {
                 using (var mapper = EntityMapper.CreateMapper())
                 {
                     var app = await GetAppAsync(appId);
                     app.Pass();
                     var result = mapper.Update(app, a => a.Id == appId);
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    var app = await GetAppAsync(appId);
                    app.Deny();
                    var result = mapper.Update(app, a => a.Id == appId);
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        #region 取消之前的推荐应用
                        {
                            var app = new App().CancelRecommand();
                            var result = mapper.Update(app, a => a.IsRecommand);
                            if (!result)
                            {
                                throw new BusinessException("取消之前的推荐应用失败");
                            }
                        }
                        #endregion

                        #region 设置新的推荐应用
                        {
                            var app = new App().Recommand();
                            var result = mapper.Update(app, a => a.Id == appId);
                            if (!result)
                            {
                                throw new BusinessException("设置新的推荐应用失败");
                            }
                        }
                        #endregion

                        mapper.Commit();
                    }
                    catch (Exception)
                    {
                        mapper.Rollback();
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        #region 移除应用的评分
                        {
                            var appStar = new AppStar();
                            appStar.Remove();
                            var result = mapper.Update(appStar, star => star.AppId == appId);
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
                            var result = mapper.Update(app, a => a.Id == appId);
                            if (!result)
                            {
                                throw new BusinessException("移除应用失败");
                            }
                        }
                        #endregion

                        mapper.Commit();
                    }
                    catch (Exception)
                    {
                        mapper.Rollback();
                        throw;
                    }
                }
            });
        }

        public async Task<App> ReleaseAppAsync(Int32 appId)
        {
            Parameter.Validate(appId);
            return await Task.Run<App>(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    #region 发布应用
                    {
                        var app = new App().AppRelease().Pass();
                        var result = mapper.Update(app, a => a.Id == appId);
                        if (!result)
                        {
                            throw new BusinessException("发布应用失败");
                        }
                    }
                    #endregion

                    #region 获取应用名称
                    {
                        return mapper.Query<App>().Where(a => a.Id == appId).Select(a => new { a.Name, a.UserId }).FirstOrDefault();
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
                using (var mapper = EntityMapper.CreateMapper())
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
                    var result = mapper.Update(app, a => a.UserId == userId && a.Id == app.Id);
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    #region 前置条件验证
                    {
                        var result = mapper.Query<App>().Where(a => a.AppTypeId == appTypeId).Count();
                        if (result > 0)
                        {
                            throw new BusinessException($@"当前分类下存在应用,不能删除当前分类");
                        }

                    }
                    #endregion

                    #region 移除应用分类
                    {
                        var appType = new AppType();
                        appType.Remove();
                        var result = mapper.Update(appType, type => type.Id == appTypeId);
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    #region 前置条件验证
                    {
                        var result = mapper.Query<AppType>().Where(a => a.Name == appType.Name).Count();
                        if (result > 0)
                        {
                            throw new BusinessException($@"分类:{appType.Name},已存在");
                        }
                    }
                    #endregion

                    #region 添加应用分类
                    {
                        var result = mapper.Add(appType);
                        if (result.Id <= 0)
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
                using (var mapper = EntityMapper.CreateMapper())
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
                        var result = mapper.Update(appType, type => type.Id == appTypeId);
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    var app = new App();
                    app.ModifyIconUrl(newIcon);
                    var result = mapper.Update(app, a => a.Id == appId && a.UserId == userId);
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        App app = null;
                        #region 获取app
                        {
                            app = mapper.Query<App>().Where(w => w.AppAuditState == AppAuditState.Pass && w.AppReleaseState == AppReleaseState.Release && w.Id == appId)
                            .Select(a => new
                            {
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
                            }).FirstOrDefault();
                            if (app == null)
                            {
                                throw new BusinessException($"获取应用失败，请刷新重试");
                            }
                        }
                        #endregion

                        #region 添加桌面应用
                        {
                            var newMember = new Member(app.Name, app.IconUrl, app.AppUrl, app.Id, app.Width, app.Height, userId, deskNum, app.AppStyle, app.IsIconByUpload, app.IsSetbar, app.IsOpenMax, app.IsFlash, app.IsResize);
                            mapper.Add(newMember);
                        }
                        #endregion

                        #region 更改应用使用数量
                        {
                            app.IncreaseUseCount();
                            var result = mapper.Update(app, a => a.Id == appId);
                            if (!result)
                            {
                                throw new BusinessException("修改应用使用数量失败");
                            }
                        }
                        #endregion

                        mapper.Commit();
                        return app;
                    }
                    catch (Exception)
                    {
                        mapper.Rollback();
                        throw;
                    }
                }
            });
        }
    }
}
