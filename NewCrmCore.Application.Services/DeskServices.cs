using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.NotifyCenter;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
    public class DeskServices : IDeskServices
    {
        private readonly IMemberContext _memberContext;

        private readonly IDeskContext _deskContext;

        private readonly CommonNotify _commonNotify;

        public DeskServices(IMemberContext memberContext, IDeskContext deskContext, CommonNotify commonNotify)
        {
            _memberContext = memberContext;
            _deskContext = deskContext;
            _commonNotify = commonNotify;
        }

        public async Task<MemberDto> GetMemberAsync(Int32 userId, Int32 memberId, Boolean isFolder)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
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
            Parameter.IfNullOrZero(userId);

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
                        appId = member.Id,
                        name = member.Name,
                        icon = member.IsIconByUpload ? $@"{Appsetting.FileUrl}{member.IconUrl}" : member.IconUrl,
                        width = member.Width,
                        height = member.Height,
                        isOnDock = member.IsOnDock,
                        isOpenMax = member.IsOpenMax,
                        isSetbar = member.IsSetbar,
                        apps = members.Where(m => m.FolderId == member.Id).Select(app => new
                        {
                            type = app.MemberType.ToString().ToLower(),
                            memberId = app.Id,
                            appId = app.Id,
                            name = app.Name,
                            icon = app.IsIconByUpload ? $@"{Appsetting.FileUrl }{app.IconUrl}" : app.IconUrl,
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
                        appId = member.Id,
                        name = member.Name,
                        icon = member.IsIconByUpload ? $@"{Appsetting.FileUrl}{ member.IconUrl}" : member.IconUrl,
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
            Parameter.IfNullOrZero(name);
            return await _memberContext.CheckMemberNameAsync(name);
        }

        public async Task ModifyDefaultDeskNumberAsync(Int32 userId, Int32 newDefaultDeskNumber)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(newDefaultDeskNumber);
            await _deskContext.ModifyDefaultDeskNumberAsync(userId, newDefaultDeskNumber);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task ModifyDockPositionAsync(Int32 userId, Int32 defaultDeskNumber, String newPosition)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(defaultDeskNumber);
            Parameter.IfNullOrZero(newPosition);
            await _deskContext.ModifyDockPositionAsync(userId, defaultDeskNumber, newPosition);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task MemberInDockAsync(Int32 userId, Int32 memberId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            await _deskContext.MemberInDockAsync(userId, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task MemberOutDockAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(deskId);
            await _deskContext.MemberOutDockAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DockToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(folderId);
            await _deskContext.DockToFolderAsync(userId, memberId, folderId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task FolderToDockAsync(Int32 userId, Int32 memberId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);

            await _deskContext.FolderToDockAsync(userId, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DeskToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(folderId);
            await _deskContext.DeskToFolderAsync(userId, memberId, folderId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task FolderToDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(deskId);
            await _deskContext.FolderToDeskAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task FolderToOtherFolderAsync(Int32 userId, Int32 memberId, Int32 folderId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(folderId);
            await _deskContext.FolderToOtherFolderAsync(userId, memberId, folderId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DeskToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(deskId);
            await _deskContext.DeskToOtherDeskAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyFolderInfoAsync(Int32 userId, String memberName, String memberIcon, Int32 memberId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberName);
            Parameter.IfNullOrZero(memberIcon);
            Parameter.IfNullOrZero(memberId);

            await _memberContext.ModifyFolderInfoAsync(userId, memberName, memberIcon, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task UninstallMemberAsync(Int32 userId, Int32 memberId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            var app = await _memberContext.UninstallMemberAsync(userId, memberId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
            if (app != null)
            {
                await _commonNotify.SendNotify(userId, new Notify("应用卸载提醒", $@"您安装的应用 {app.Name} 已卸载完成", 0, userId));
            }
        }

        public async Task ModifyMemberInfoAsync(Int32 userId, MemberDto memberDto)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberDto);

            var member = await _memberContext.GetMemberAsync(userId, memberDto.Id, memberDto.MemberType == MemberType.Folder);

            member.ModifyIconUrl(memberDto.IconUrl);
            member.ModifyName(memberDto.Name);
            member.ModifyWidth(memberDto.Width);
            member.ModifyHeight(memberDto.Height);

            member = memberDto.IsIconByUpload ? member.IconFromUpload() : member.IconNotFromUpload();
            member = memberDto.IsResize ? member.Resize() : member.NotResize();
            member = memberDto.IsOpenMax ? member.OpenMax() : member.NotOpenMax();
            member = memberDto.IsFlash ? member.Flash() : member.NotFlash();

            await _memberContext.ModifyMemberInfoAsync(userId, member);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task CreateNewFolderAsync(String folderName, String folderImg, Int32 deskId, Int32 userId)
        {
            Parameter.IfNullOrZero(folderName);
            Parameter.IfNullOrZero(folderImg);
            Parameter.IfNullOrZero(deskId);
            Parameter.IfNullOrZero(userId);
            await _deskContext.CreateNewFolderAsync(deskId, folderName, folderImg, userId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task DockToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(deskId);
            await _deskContext.DockToOtherDeskAsync(userId, memberId, deskId);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyMemberIconAsync(Int32 userId, Int32 memberId, String newIcon)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(memberId);
            Parameter.IfNullOrZero(newIcon);
            await _memberContext.ModifyMemberIconAsync(userId, memberId, newIcon);
            await CacheHelper.RemoveKeyWhenModify(new DesktopCacheKey(userId));
        }

        public async Task ModifyWallpaperSourceAsync(String source, Int32 userId)
        {
            Parameter.IfNullOrZero(source);
            Parameter.IfNullOrZero(userId);
            await _deskContext.ModifyWallpaperSourceAsync(source, userId);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task<PageList<NotifyDto>> CheckUnreadNotifyCount(Int32 userId, Int32 pageIndex, Int32 pageSize)
        {
            Parameter.IfNullOrZero(pageIndex);
            Parameter.IfNullOrZero(pageSize);
            return await Task.Run(() =>
            {
                var result = _deskContext.CheckUnreadNotifyCount(userId, pageIndex, pageSize, out int totalCount);
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
            Parameter.IfNullOrZero(notifyIds);
            await _deskContext.ReadNotify(notifyIds);
        }

        public async Task<IDictionary<String, dynamic>> GetAllSkinAsync(String skinPath)
        {
            Parameter.IfNullOrZero(skinPath);

            return await Task.Run(() =>
            {
                IDictionary<String, dynamic> dataDictionary = new Dictionary<String, dynamic>();
                Directory.GetFiles(skinPath, "*.css").ToList().ForEach(path =>
                {
                    var fileName = Path.GetFileName(path);
                    fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                    var cssPath = path[(path.LastIndexOf("images") - 1)..].Replace(@"\", "/");
                    dataDictionary.Add(fileName, new
                    {
                        cssPath,
                        imgPath = $@"{cssPath.Substring(0, cssPath.LastIndexOf("."))}/preview.png"
                    });
                });
                return dataDictionary;
            });
        }

        public async Task ModifySkinAsync(Int32 userId, String newSkin)
        {
            Parameter.IfNullOrZero(userId);
            Parameter.IfNullOrZero(newSkin);
            await _deskContext.ModifySkinAsync(userId, newSkin);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

    }
}
