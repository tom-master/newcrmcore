using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Services;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools.CustomException;
using NewLibCore;
using NewLibCore.Security;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
	public class AccountServices: IAccountServices
	{
		private readonly IAccountContext _accountContext;

		public AccountServices(IAccountContext accountContext)
		{
			_accountContext = accountContext;
		}

		public async Task<AccountDto> LoginAsync(String accountName, String password, String requestIp)
		{
			new Parameter().Validate(accountName).Validate(password).Validate(requestIp);

			var account = await _accountContext.ValidateAsync(accountName, password, requestIp);
			return new AccountDto
			{
				Name = account.Name,
				Id = account.Id,
				AccountFace = account.AccountFace
			};
		}

		public async Task<ConfigDto> GetConfigAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);

			var config = await CacheHelper.GetCache(new ConfigCacheKey(accountId), () => _accountContext.GetConfigAsync(accountId));
			var wallpaper = await CacheHelper.GetCache(new WallpaperCacheKey(accountId), () => _accountContext.GetWallpaperAsync(config.WallpaperId));

			return new ConfigDto
			{
				Id = config.Id,
				Skin = config.Skin,
				AccountFace = config.AccountFace,
				AppSize = config.AppSize,
				AppVerticalSpacing = config.AppVerticalSpacing,
				AppHorizontalSpacing = config.AppHorizontalSpacing,
				DefaultDeskNumber = config.DefaultDeskNumber,
				DefaultDeskCount = config.DefaultDeskCount,
				AppXy = config.AppXy.ToString().ToLower(),
				DockPosition = config.DockPosition.ToString().ToLower(),
				WallpaperUrl = wallpaper.Url,
				WallpaperWidth = wallpaper.Width,
				WallpaperHeigth = wallpaper.Height,
				WallpaperSource = wallpaper.Source.ToString().ToLower(),
				WallpaperMode = config.WallpaperMode.ToString().ToLower(),
				IsBing = config.IsBing,
				AccountId = config.AccountId
			};
		}

		public List<AccountDto> GetAccounts(String accountName, String accountType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
		{
			new Parameter().Validate(accountName).Validate(accountType);

			var result = _accountContext.GetAccounts(accountName, accountType, pageIndex, pageSize, out totalCount);

			return result.Select(s => new AccountDto
			{
				Id = s.Id,
				IsAdmin = s.IsAdmin,
				Name = s.Name,
				AccountFace = ProfileManager.FileUrl + s.AccountFace,
				IsDisable = s.IsDisable
			}).ToList();
		}

		public async Task<AccountDto> GetAccountAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);

			var account = await CacheHelper.GetCache(new AccountCacheKey(accountId), () => _accountContext.GetAccountAsync(accountId));
			if (account == null)
			{
				throw new BusinessException("该用户可能已被禁用或被删除，请联系管理员");
			}

			var roles = await CacheHelper.GetCache(new AccountRoleCacheKey(account.Id), () => _accountContext.GetRolesAsync(account.Id));
			var powers = await CacheHelper.GetCache(new PowersCacheKey(account.Id), () => _accountContext.GetPowersAsync());

			return new AccountDto
			{
				AccountFace = account.AccountFace,
				AddTime = account.AddTime.ToString("yyyy-MM-dd"),
				Id = account.Id,
				IsAdmin = account.IsAdmin,
				IsDisable = account.IsDisable,
				IsOnline = account.IsOnline,
				LastLoginTime = account.LastLoginTime.ToString("yyyy-MM-dd"),
				LastModifyTime = account.LastModifyTime.ToString("yyyy-MM-dd"),
				LockScreenPassword = account.LockScreenPassword,
				Password = account.LoginPassword,
				Name = account.Name,
				Roles = roles.Select(s => new RoleDto
				{
					Id = s.Id,
					Name = s.Name,
					Powers = powers.Where(w => w.RoleId == s.Id).Select(p => new PowerDto
					{
						Id = p.AppId
					}).ToList(),
					RoleIdentity = s.RoleIdentity
				}).ToList()
			};
		}

		public async Task<Boolean> CheckAccountNameExistAsync(String accountName)
		{
			new Parameter().Validate(accountName);
			return await _accountContext.CheckAccountNameExistAsync(accountName);
		}

		public async Task<Boolean> CheckPasswordAsync(Int32 accountId, String oldAccountPassword)
		{
			new Parameter().Validate(accountId).Validate(oldAccountPassword);
			var result = await _accountContext.GetOldPasswordAsync(accountId);
			return PasswordUtil.ComparePasswords(result, oldAccountPassword);
		}

		public async Task<Boolean> UnlockScreenAsync(Int32 accountId, String unlockPassword)
		{
			new Parameter().Validate(accountId).Validate(unlockPassword);
			return await _accountContext.UnlockScreenAsync(accountId, unlockPassword);
		}

		public async Task<Boolean> CheckAppNameAsync(String name)
		{
			new Parameter().Validate(name);
			return await _accountContext.CheckAppNameAsync(name);
		}

		public async Task<Boolean> CheckAppUrlAsync(String url)
		{
			new Parameter().Validate(url);
			return await _accountContext.CheckAppUrlAsync(url);
		}

		public async Task AddNewAccountAsync(AccountDto accountDto)
		{
			new Parameter().Validate(accountDto);

			var account = accountDto.ConvertToModel<AccountDto, Account>();

			var accountType = EnumExtensions.ToEnum<AccountType>(account.IsAdmin ? 2 /*管理员*/ : 1 /*用户*/);
			var newPassword = PasswordUtil.CreateDbPassword(account.LoginPassword);

			var internalNewAccount = new Account(account.Name, newPassword, account.Roles, accountType);
			await _accountContext.AddNewAccountAsync(internalNewAccount);
		}

		public async Task ModifyAccountAsync(AccountDto accountDto)
		{
			new Parameter().Validate(accountDto);

			var account = accountDto.ConvertToModel<AccountDto, Account>();
			await _accountContext.ModifyAccountAsync(account);
		}

		public async Task LogoutAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			await _accountContext.LogoutAsync(accountId);
		}

		public async Task EnableAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			await _accountContext.EnableAsync(accountId);
		}

		public async Task DisableAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			await _accountContext.DisableAsync(accountId);
		}

		public async Task ModifyAccountFaceAsync(Int32 accountId, String newFace)
		{
			new Parameter().Validate(accountId).Validate(newFace);

			await _accountContext.ModifyAccountFaceAsync(accountId, newFace);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId), new AccountCacheKey(accountId));
		}

		public async Task ModifyPasswordAsync(Int32 accountId, String newPassword, Boolean isTogetherSetLockPassword)
		{
			new Parameter().Validate(newPassword);

			var password = PasswordUtil.CreateDbPassword(newPassword);
			await _accountContext.ModifyPasswordAsync(accountId, password, isTogetherSetLockPassword);
		}

		public async Task ModifyLockScreenPasswordAsync(Int32 accountId, String newScreenPassword)
		{
			new Parameter().Validate(newScreenPassword);

			var newPassword = PasswordUtil.CreateDbPassword(newScreenPassword);
			await _accountContext.ModifyLockScreenPasswordAsync(accountId, newPassword);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task RemoveAccountAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			await _accountContext.RemoveAccountAsync(accountId);
		}
	}
}
