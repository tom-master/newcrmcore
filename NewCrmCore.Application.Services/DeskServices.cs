using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools.CustomException;
using NewLibCore;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
	public class DeskServices: IDeskServices
	{
		private readonly IMemberContext _memberContext;
		private readonly IDeskContext _deskContext;

		public DeskServices(IMemberContext memberContext, IDeskContext deskContext)
		{
			_memberContext = memberContext;
			_deskContext = deskContext;
		}

		public async Task<MemberDto> GetMemberAsync(Int32 accountId, Int32 memberId, Boolean isFolder)
		{
			new Parameter().Validate(accountId).Validate(memberId);
			var result = await _memberContext.GetMemberAsync(accountId, memberId, isFolder);
			if (result == null)
			{
				throw new BusinessException($"未找到app");
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
				IsDraw = result.IsDraw,
				IsFlash = result.IsFlash,
				IsFull = result.IsFull,
				IsLock = result.IsLock,
				IsMax = result.IsMax,
				IsOnDock = result.IsOnDock,
				IsOpenMax = result.IsOpenMax,
				IsResize = result.IsResize,
				IsSetbar = result.IsSetbar,
				MemberType = result.MemberType.ToString(),
				Name = result.Name,
				Width = result.Width,
				AccountId = result.AccountId,
				IsIconByUpload = result.IsIconByUpload
			};
		}

		public async Task<IDictionary<String, IList<dynamic>>> GetDeskMembersAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);

			var result = await CacheHelper.GetCache(new DesktopCacheKey(accountId), () => _memberContext.GetMembersAsync(accountId));
			var deskGroup = result.GroupBy(a => a.DeskIndex);
			var deskDictionary = new Dictionary<String, IList<dynamic>>();
			foreach (var desk in deskGroup)
			{
				var members = desk.ToList();
				var deskMembers = new List<dynamic>();
				foreach (var member in members)
				{
					var internalType = member.MemberType.ToString().ToLower();

					if (member.MemberType == MemberType.Folder)
					{
						deskMembers.Add(new
						{
							type = internalType,
							memberId = member.Id,
							appId = member.AppId,
							name = member.Name,
							icon = member.IsIconByUpload ? ProfileManager.FileUrl + member.IconUrl : member.IconUrl,
							width = member.Width,
							height = member.Height,
							isOnDock = member.IsOnDock,
							isDraw = member.IsDraw,
							isOpenMax = member.IsOpenMax,
							isSetbar = member.IsSetbar,
							apps = members.Where(m => m.FolderId == member.Id).Select(app => new
							{
								type = app.MemberType.ToString().ToLower(),
								memberId = app.Id,
								appId = app.AppId,
								name = app.Name,
								icon = member.IsIconByUpload ? ProfileManager.FileUrl + member.IconUrl : member.IconUrl,
								width = app.Width,
								height = app.Height,
								isOnDock = app.IsOnDock,
								isDraw = app.IsDraw,
								isOpenMax = app.IsOpenMax,
								isSetbar = app.IsSetbar,
							})
						});
					}
					else
					{
						if (member.FolderId == 0)
						{
							deskMembers.Add(new
							{
								type = internalType,
								memberId = member.Id,
								appId = member.AppId,
								name = member.Name,
								icon = member.IsIconByUpload ? ProfileManager.FileUrl + member.IconUrl : member.IconUrl,
								width = member.Width,
								height = member.Height,
								isOnDock = member.IsOnDock,
								isDraw = member.IsDraw,
								isOpenMax = member.IsOpenMax,
								isSetbar = member.IsSetbar
							});
						}
					}
				}
				deskDictionary.Add(desk.Key.ToString(), deskMembers);
			}

			return deskDictionary;
		}

		public async Task<Boolean> CheckMemberNameAsync(String name)
		{
			new Parameter().Validate(name);
			return await _memberContext.CheckMemberNameAsync(name);
		}

		public async Task ModifyDefaultDeskNumberAsync(Int32 accountId, Int32 newDefaultDeskNumber)
		{
			new Parameter().Validate(accountId).Validate(newDefaultDeskNumber);
			await _deskContext.ModifyDefaultDeskNumberAsync(accountId, newDefaultDeskNumber);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task ModifyDockPositionAsync(Int32 accountId, Int32 defaultDeskNumber, String newPosition)
		{
			new Parameter().Validate(accountId).Validate(defaultDeskNumber).Validate(newPosition);
			await _deskContext.ModifyDockPositionAsync(accountId, defaultDeskNumber, newPosition);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task MemberInDockAsync(Int32 accountId, Int32 memberId)
		{
			new Parameter().Validate(accountId).Validate(memberId);
			await _deskContext.MemberInDockAsync(accountId, memberId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task MemberOutDockAsync(Int32 accountId, Int32 memberId, Int32 deskId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
			await _deskContext.MemberOutDockAsync(accountId, memberId, deskId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task DockToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(folderId);
			await _deskContext.DockToFolderAsync(accountId, memberId, folderId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task FolderToDockAsync(Int32 accountId, Int32 memberId)
		{
			new Parameter().Validate(accountId).Validate(memberId);
			await _deskContext.FolderToDockAsync(accountId, memberId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task DeskToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(folderId);
			await _deskContext.DeskToFolderAsync(accountId, memberId, folderId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task FolderToDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
			await _deskContext.FolderToDeskAsync(accountId, memberId, deskId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task FolderToOtherFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(folderId);
			await _deskContext.FolderToOtherFolderAsync(accountId, memberId, folderId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task DeskToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
			await _deskContext.DeskToOtherDeskAsync(accountId, memberId, deskId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task ModifyFolderInfoAsync(Int32 accountId, String memberName, String memberIcon, Int32 memberId)
		{
			new Parameter().Validate(accountId).Validate(memberName).Validate(memberIcon).Validate(memberId);
			await _memberContext.ModifyFolderInfoAsync(accountId, memberName, memberIcon, memberId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task UninstallMemberAsync(Int32 accountId, Int32 memberId)
		{
			new Parameter().Validate(accountId).Validate(memberId);
			await _memberContext.UninstallMemberAsync(accountId, memberId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task ModifyMemberInfoAsync(Int32 accountId, MemberDto member)
		{
			new Parameter().Validate(accountId).Validate(member);
			await _memberContext.ModifyMemberInfoAsync(accountId, member.ConvertToModel<MemberDto, Member>());
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task CreateNewFolderAsync(String folderName, String folderImg, Int32 deskId, Int32 accountId)
		{
			new Parameter().Validate(folderName).Validate(folderImg).Validate(deskId).Validate(accountId);
			await _deskContext.CreateNewFolderAsync(deskId, folderName, folderImg, accountId);
		}

		public async Task DockToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(deskId);
			await _deskContext.DockToOtherDeskAsync(accountId, memberId, deskId);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task ModifyMemberIconAsync(Int32 accountId, Int32 memberId, String newIcon)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(newIcon);
			await _memberContext.ModifyMemberIconAsync(accountId, memberId, newIcon);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task ModifyWallpaperSourceAsync(String source, Int32 accountId)
		{
			new Parameter().Validate(source).Validate(accountId);
			await _deskContext.ModifyWallpaperSourceAsync(source, accountId);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}
	}
}
