using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure;
using NewLibCore;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class DeskContext : IDeskContext
    {
        public async Task ModifyDefaultDeskNumberAsync(Int32 accountId, Int32 newDefaultDeskNumber)
        {
            new Parameter().Validate(accountId).Validate(newDefaultDeskNumber);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyDefaultDeskNumber(newDefaultDeskNumber);
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task ModifyDockPositionAsync(Int32 accountId, Int32 defaultDeskNumber, String position)
        {
            new Parameter().Validate(accountId).Validate(defaultDeskNumber).Validate(position);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    var newPosition = EnumExtensions.ToEnum<DockPosition>(position);
                    config.PositionTo(newPosition);
                    dataStore.Modify(config, conf => conf.AccountId == accountId && conf.DefaultDeskNumber == defaultDeskNumber);
                }
            });
        }

        public async Task ModifyMemberDirectionToXAsync(Int32 accountId)
        {
            new Parameter().Validate(accountId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.DirectionToX();
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task ModifyMemberDirectionToYAsync(Int32 accountId)
        {
            new Parameter().Validate(accountId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.DirectionToY();
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task ModifyMemberDisplayIconSizeAsync(Int32 accountId, Int32 newSize)
        {
            new Parameter().Validate(accountId).Validate(newSize);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyAppSize(newSize);
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task ModifyMemberHorizontalSpacingAsync(Int32 accountId, Int32 newSize)
        {
            new Parameter().Validate(accountId).Validate(newSize);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyAppVerticalSpacing(newSize);
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });

        }

        public async Task ModifyMemberVerticalSpacingAsync(Int32 accountId, Int32 newSize)
        {
            new Parameter().Validate(accountId).Validate(newSize);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config();
                    config.ModifyAppHorizontalSpacing(newSize);
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task MemberInDockAsync(Int32 accountId, Int32 memberId)
        {
            new Parameter().Validate(accountId).Validate(memberId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OnDock();
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task MemberOutDockAsync(Int32 accountId, Int32 memberId, Int32 deskId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OutDock().ModifyDeskIndex(deskId);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task DockToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(folderId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OutDock().ModifyFolderId(folderId);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task FolderToDockAsync(Int32 accountId, Int32 memberId)
        {
            new Parameter().Validate(accountId).Validate(memberId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OnDock().ModifyFolderId(0);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task DeskToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(folderId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyFolderId(folderId);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task FolderToDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyFolderId(0).ModifyDeskIndex(deskId);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task FolderToOtherFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(folderId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyFolderId(folderId);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task DeskToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
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
                            var sql = $@"SELECT COUNT(*) FROM Member AS a WHERE a.Id=0 AND a.AccountId=0 AND a.IsDeleted=0 AND IsOnDock=1";
                            if (dataStore.FindSingleValue<Int32>(sql) > 0)
                            {
                                member.OutDock();
                            }
                        }
                        #endregion

                        #region 成员移动到其他桌面
                        {
                            dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
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

        public async Task DockToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId)
        {
            new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.OutDock().ModifyDeskIndex(deskId);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task CreateNewFolderAsync(Int32 deskId, String folderName, String folderImg, Int32 accountId)
        {
            new Parameter().Validate(deskId).Validate(folderImg).Validate(folderName);
            await Task.Run(() =>
            {
                var folder = new Member(folderName, folderImg, 0, accountId, deskId, false);
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.Add(folder);
                }
            });
        }

        public async Task ModifyWallpaperSourceAsync(String source, Int32 accountId)
        {
            new Parameter().Validate(source).Validate(accountId);
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
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task<Int32> CheckUnreadNotifyCount(Int32 accountId)
        {
            new Parameter().Validate(accountId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM Notify AS a WHERE a.IsRead=0 AND a.ToAccountId=@Id AND a.IsDelete=0 ";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@Id",accountId)
                    };
                    return dataStore.FindSingleValue<Int32>(sql);
                }
            });
        }
    }
}
