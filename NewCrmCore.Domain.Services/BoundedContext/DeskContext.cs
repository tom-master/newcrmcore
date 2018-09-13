using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class DeskContext : IDeskContext
    {
        public async Task ModifyDefaultDeskNumberAsync(Int32 userId, Int32 newDefaultDeskNumber)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newDefaultDeskNumber);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyDefaultDeskNumber(newDefaultDeskNumber);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改默认桌面号失败");
                    }
                }
            });
        }

        public async Task ModifyDockPositionAsync(Int32 userId, Int32 defaultDeskNumber, String position)
        {
            Parameter.Validate(userId);
            Parameter.Validate(defaultDeskNumber);
            Parameter.Validate(position);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    var newPosition = EnumExtensions.ToEnum<DockPosition>(position);
                    config.PositionTo(newPosition);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId && conf.DefaultDeskNumber == defaultDeskNumber);
                    if (!result)
                    {
                        throw new BusinessException("修改应用码头位置失败");
                    }
                }
            });
        }

        public async Task ModifyMemberDirectionToXAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.DirectionToX();
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改桌面成员为X轴失败");
                    }
                }
            });
        }

        public async Task ModifyMemberDirectionToYAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.DirectionToY();
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改桌面成员为Y轴失败");
                    }
                }
            });
        }

        public async Task ModifyMemberDisplayIconSizeAsync(Int32 userId, Int32 newSize)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newSize);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyAppSize(newSize);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改桌面成员展示图标大小失败");
                    }
                }
            });
        }

        public async Task ModifyMemberHorizontalSpacingAsync(Int32 userId, Int32 newSize)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newSize);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyAppVerticalSpacing(newSize);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改桌面成员水平间距失败");
                    }
                }
            });
        }

        public async Task ModifyMemberVerticalSpacingAsync(Int32 userId, Int32 newSize)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newSize);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyAppHorizontalSpacing(newSize);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改桌面成员垂直间距失败");
                    }
                }
            });
        }

        public async Task MemberInDockAsync(Int32 userId, Int32 memberId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OnDock();
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("将成员移动到应用码头失败");
                    }
                }
            });
        }

        public async Task MemberOutDockAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OutDock().ModifyDeskIndex(deskId);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("将桌面成员移出应用码头失败");
                    }
                }
            });
        }

        public async Task DockToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(folderId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OutDock().ModifyFolderId(folderId);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("桌面成员从应用码头移动到文件夹中失败");
                    }
                }
            });
        }

        public async Task FolderToDockAsync(Int32 userId, Int32 memberId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OnDock().ModifyFolderId(0);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("桌面成员从文件夹移动到码头失败");
                    }
                }
            });
        }

        public async Task DeskToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(folderId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyFolderId(folderId);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("桌面成员从桌面移动到文件夹中");
                    }
                }
            });
        }

        public async Task FolderToDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyFolderId(0).ModifyDeskIndex(deskId);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("桌面成员从文件夹移动到桌面失败");
                    }

                }
            });
        }

        public async Task FolderToOtherFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(folderId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyFolderId(folderId);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("桌面成员从文件夹移动到另一个文件夹中失败");
                    }
                }
            });
        }

        public async Task DeskToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        var member = new Member();
                        member.ModifyDeskIndex(deskId);

                        var set = new StringBuilder();
                        #region 查询成员是否在应用码头中
                        {
                            var sql = $@"SELECT COUNT(*) FROM Member AS a WHERE a.Id=0 AND a.UserId=0 AND a.IsDeleted=0 AND IsOnDock=1";
                            if (dataStore.FindSingleValue<Int32>(sql) > 0)
                            {
                                member.OutDock();
                            }
                        }
                        #endregion

                        #region 成员移动到其他桌面
                        {
                            var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                            if (!result)
                            {
                                throw new BusinessException("成员移动到其他桌面失败");
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

        public async Task DockToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OutDock().ModifyDeskIndex(deskId);
                    var result = dataStore.Modify(member, mem => mem.Id == memberId && mem.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("桌面成员从应用码头移动到另一桌面时失败");
                    }
                }
            });
        }

        public async Task CreateNewFolderAsync(Int32 deskId, String folderName, String folderImg, Int32 userId)
        {
            Parameter.Validate(deskId);
            Parameter.Validate(folderImg);
            Parameter.Validate(folderName);

            await Task.Run(() =>
            {
                var folder = new Member(folderName, folderImg, 0, userId, deskId, false);
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.Add(folder);
                }
            });
        }

        public async Task ModifyWallpaperSourceAsync(String source, Int32 userId)
        {
            Parameter.Validate(source);
            Parameter.Validate(userId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    if (source.ToLower() == WallpaperSource.Bing.ToString().ToLower())
                    {
                        config.FromBing();
                    }
                    else
                    {
                        config.NotFromBing();
                    }
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改壁纸来源失败");
                    }
                }
            });
        }

        public IList<Notify> CheckUnreadNotifyCount(Int32 userId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userId);
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);
            using (var dataStore = new DataStore(Appsetting.Database))
            {
                var parameters = new List<ParameterMapper>
                {
                    new ParameterMapper("@userId",userId)
                };
                var pageModel = new PageList<Notify>();
                {
                    var sql = $@"SELECT COUNT(*) FROM Notify AS a WHERE a.IsDeleted=0 AND a.ToUserId=@userId";
                    totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
                }

                {
                    var sql = $@"SELECT a.Id,a.Title,a.Content,a.IsRead,a.ToUserId FROM Notify AS a WHERE a.IsDeleted=0 AND a.ToUserId=@userId
                                 LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    return dataStore.Find<Notify>(sql, parameters);
                }
            }
        }

        public async Task ReadNotify(IList<Int32> notifyIds)
        {
            Parameter.Validate(notifyIds);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var notify = new Notify();
                    notify.Read();
                    var result = dataStore.Modify(notify, n => notifyIds.Contains(n.Id));
                    if (!result)
                    {
                        throw new BusinessException("读取消息失败");
                    }
                }
            });
        }

        public async Task ModifySkinAsync(Int32 userId, String newSkin)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newSkin);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifySkin(newSkin);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改皮肤失败");
                    }
                }
            });
        }
    }
}
