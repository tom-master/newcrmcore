using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
    public class DeskServices : IDeskServices
    {
        private readonly IMemberContext _memberContext;
        private readonly IDeskContext _deskContext;

        public DeskServices(IMemberContext memberContext, IDeskContext deskContext)
        {
            _memberContext = memberContext;
            _deskContext = deskContext;
        }

        public async Task<MemberDto> GetMemberAsync(Int32 userId, Int32 memberId, Boolean isFolder)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            var result = await _memberContext.GetMemberAsync(userId, memberId, isFolder);
            if (result == null)
            {
                throw new BusinessException($"未能查找到所请求的应用");
            }

            return new MemberDto
            {
                AppId = result.AppId,
                AppUrl = result.AppUrl,
                DeskIndex = result.DeskIndex,
                FolderId = result.FolderId,
                Height = result.Height,
                IconUrl = result.IconUrl,
                Id = result.Id,
                IsFlash = result.IsFlash,
                IsOnDock = result.IsOnDock,
                IsOpenMax = result.IsOpenMax,
                IsResize = result.IsResize,
                IsSetbar = result.IsSetbar,
                MemberType = result.MemberType,
                Name = result.Name,
                Width = result.Width,
                UserId = result.UserId,
                IsIconByUpload = result.IsIconByUpload,
                StarCount = result.StarCount
            };
        }

        public async Task<IDictionary<String, IList<dynamic>>> GetDeskMembersAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            var result = await CacheHelper.GetOrSetCacheAsync(new DesktopCacheKey(userId), () => _memberContext.GetMembersAsync(userId));
            var deskGroup = result.GroupBy(a => a.DeskIndex);
            var deskDictionary = new Dictionary<String, IList<dynamic>>();
            foreach (var desk in deskGroup)
            {
                var members = desk.ToList();
                var deskMembers = new List<dynamic>();
                foreach (var member in members.Where(w => w.MemberType == MemberType.Folder))
                {
                    deskMembers.Add(new
                    {
                        type = member.MemberType.ToString().ToLower(),
                        memberId = member.Id,
                        appId = member.AppId,
                        name = member.Name,
                        icon = member.IsIconByUpload ? Appsetting.FileUrl + member.IconUrl : member.IconUrl,
                        width = member.Width,
                        height = member.Height,
                        isOnDock = member.IsOnDock,
                        isOpenMax = member.IsOpenMax,
                        isSetbar = member.IsSetbar,
                        apps = members.Where(m => m.FolderId == member.Id).Select(app => new
                        {
                            type = app.MemberType.ToString().ToLower(),
                            memberId = app.Id,
                            appId = app.AppId,
                            name = app.Name,
                            icon = app.IsIconByUpload ? Appsetting.FileUrl + app.IconUrl : app.IconUrl,
                            width = app.Width,
                            height = app.Height,
                            isOnDock = app.IsOnDock,
                            isOpenMax = app.IsOpenMax,
                            isSetbar = app.IsSetbar,
                        })
                    });
                }
                foreach (var member in members.Where(w => (w.MemberType == MemberType.App || w.MemberType == MemberType.Widget) && w.FolderId == 0))
                {
                    deskMembers.Add(new
                    {
                        type = member.MemberType.ToString().ToLower(),
                        memberId = member.Id,
                        appId = member.AppId,
                        name = member.Name,
                        icon = member.IsIconByUpload ? Appsetting.FileUrl + member.IconUrl : member.IconUrl,
                        width = member.Width,
                        height = member.Height,
                        isOnDock = member.IsOnDock,
                        isOpenMax = member.IsOpenMax,
                        isSetbar = member.IsSetbar
                    });
                }
                deskDictionary.Add(desk.Key.ToString(), deskMembers);
            }

            return deskDictionary;
        }

        public async Task<Boolean> CheckMemberNameAsync(String name)
        {
            Parameter.Validate(name);
            return await _memberContext.CheckMemberNameAsync(name);
        }

        public async Task ModifyDefaultDeskNumberAsync(Int32 userId, Int32 newDefaultDeskNumber)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newDefaultDeskNumber);
            await _deskContext.ModifyDefaultDeskNumberAsync(userId, newDefaultDeskNumber);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task ModifyDockPositionAsync(Int32 userId, Int32 defaultDeskNumber, String newPosition)
        {
            Parameter.Validate(userId);
            Parameter.Validate(defaultDeskNumber);
            Parameter.Validate(newPosition);
            await _deskContext.ModifyDockPositionAsync(userId, defaultDeskNumber, newPosition);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task MemberInDockAsync(Int32 userId, Int32 memberId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            await _deskContext.MemberInDockAsync(userId, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task MemberOutDockAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);
            await _deskContext.MemberOutDockAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DockToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(folderId);
            await _deskContext.DockToFolderAsync(userId, memberId, folderId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task FolderToDockAsync(Int32 userId, Int32 memberId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);

            await _deskContext.FolderToDockAsync(userId, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DeskToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(folderId);
            await _deskContext.DeskToFolderAsync(userId, memberId, folderId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task FolderToDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);
            await _deskContext.FolderToDeskAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task FolderToOtherFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(folderId);
            await _deskContext.FolderToOtherFolderAsync(userId, memberId, folderId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DeskToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);
            await _deskContext.DeskToOtherDeskAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyFolderInfoAsync(Int32 userId, String memberName, String memberIcon, Int32 memberId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberName);
            Parameter.Validate(memberIcon);
            Parameter.Validate(memberId);

            await _memberContext.ModifyFolderInfoAsync(userId, memberName, memberIcon, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task UninstallMemberAsync(Int32 userId, Int32 memberId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            await _memberContext.UninstallMemberAsync(userId, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyMemberInfoAsync(Int32 userId, MemberDto member)
        {
            Parameter.Validate(userId);
            Parameter.Validate(member);
            await _memberContext.ModifyMemberInfoAsync(userId, member.ConvertToModel<MemberDto, Member>());
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task CreateNewFolderAsync(String folderName, String folderImg, Int32 deskId, Int32 userId)
        {
            Parameter.Validate(folderName);
            Parameter.Validate(folderImg);
            Parameter.Validate(deskId);
            Parameter.Validate(userId);
            await _deskContext.CreateNewFolderAsync(deskId, folderName, folderImg, userId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DockToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(deskId);
            await _deskContext.DockToOtherDeskAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyMemberIconAsync(Int32 userId, Int32 memberId, String newIcon)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);
            Parameter.Validate(newIcon);
            await _memberContext.ModifyMemberIconAsync(userId, memberId, newIcon);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyWallpaperSourceAsync(String source, Int32 userId)
        {
            Parameter.Validate(source);
            Parameter.Validate(userId);
            await _deskContext.ModifyWallpaperSourceAsync(source, userId);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task<PageList<NotifyDto>> CheckUnreadNotifyCount(Int32 userId, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);
            return await Task.Run(() =>
            {
                var totalCount = 0;
                var result = _deskContext.CheckUnreadNotifyCount(userId, pageIndex, pageSize, out totalCount);
                return new PageList<NotifyDto>
                {
                    TotalCount = totalCount,
                    Models = result.Select(s => new NotifyDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Content = s.Content,
                        IsNotify = s.IsNotify,
                        IsRead = s.IsRead,
                        UserId = s.UserId,
                        ToUserId = s.ToUserId
                    }).ToList()
                };
            });
        }

        public async Task ReadNotify(IList<Int32> notifyIds)
        {
            Parameter.Validate(notifyIds);
            await _deskContext.ReadNotify(notifyIds);
        }
    }
}
