using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Infrastructure.CommonTools.CustomException;
using NewLibCore;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
	public class AppServices: IAppServices
	{
		private readonly IAppContext _appContext;
		private readonly IDeskContext _deskContext;

		public AppServices(IAppContext appContext, IDeskContext deskContext)
		{
			_appContext = appContext;
			_deskContext = deskContext;
		}

		public async Task<List<AppTypeDto>> GetAppTypesAsync()
		{
			var result = await CacheHelper.GetCache(new AppTypeCacheKey(), () => _appContext.GetAppTypesAsync());
			return result.Select(s => new AppTypeDto
			{
				Id = s.Id,
				Name = s.Name
			}).ToList();
		}

		public async Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);

			var result = await _appContext.GetTodayRecommendAsync(accountId);
			result.AppIcon = result.IsIconByUpload ? AppSettings.Get<Settings>().FileUrl + result.AppIcon : result.AppIcon;
			if (result == null)
			{
				return new TodayRecommendAppDto();
			}

			return result;
		}

		public async Task<Tuple<Int32, Int32>> GetAccountDevelopAppCountAndNotReleaseAppCountAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			return await _appContext.GetAccountDevelopAppCountAndNotReleaseAppCountAsync(accountId);
		}

		public async Task<PagingModel<AppDto>> GetAppsAsync(Int32 accountId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize)
		{
			new Parameter().Validate(accountId, true).Validate(orderId).Validate(searchText).Validate(pageIndex, true).Validate(pageSize);
			return await Task.Run(() =>
			{
				var result = _appContext.GetApps(accountId, appTypeId, orderId, searchText, pageIndex, pageSize, out var totalCount);
				return new PagingModel<AppDto>
				{
					TotalCount = totalCount,
					Models = result.Select(app => new AppDto
					{
						AppTypeId = app.AppTypeId,
						AccountId = app.AccountId,
						AddTime = app.AddTime.ToString("yyyy-MM-dd"),
						UseCount = app.UseCount,
						StartCount = app.StarCount,
						Name = app.Name,
						IconUrl = app.IsIconByUpload ? AppSettings.Get<Settings>().FileUrl + app.IconUrl : app.IconUrl,
						Remark = app.Remark,
						AppStyle = (Int32)app.AppStyle,
						Id = app.Id,
						IsInstall = app.IsInstall
					}).ToList()
				};
			});
		}

		public async Task<PagingModel<AppDto>> GetAccountAppsAsync(Int32 accountId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize)
		{
			new Parameter().Validate(accountId, true).Validate(searchText).Validate(appTypeId, true).Validate(appStyleId, true).Validate(pageIndex).Validate(pageSize);

			return await Task.Run(async () =>
			{
				var result = _appContext.GetAccountApps(accountId, searchText, appTypeId, appStyleId, appState, pageIndex, pageSize, out var totalCount);
				var appTypes = await GetAppTypesAsync();
				return new PagingModel<AppDto>
				{
					TotalCount = totalCount,
					Models = result.Select(app => new AppDto
					{
						Name = app.Name,
						AppStyle = (Int32)app.AppStyle,
						AppTypeName = appTypes.FirstOrDefault(appType => appType.Id == app.AppTypeId).Name,
						UseCount = app.UseCount,
						Id = app.Id,
						IconUrl = app.IconUrl,
						AppAuditState = (Int32)app.AppAuditState,
						IsRecommand = app.IsRecommand,
						AccountId = app.AccountId,
						IsIconByUpload = app.IsIconByUpload
					}).ToList()
				};
			});
		}

		public async Task<AppDto> GetAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);

			var result = await _appContext.GetAppAsync(appId);
			var appTypes = await CacheHelper.GetCache(new AppTypeCacheKey(), () => GetAppTypesAsync());

			return new AppDto
			{
				Name = result.Name,
				IconUrl = result.IconUrl,
				Remark = result.Remark,
				UseCount = result.UseCount,
				StartCount = result.StarCount,
				AppTypeName = appTypes.FirstOrDefault(appType => appType.Id == result.AppTypeId).Name,
				AddTime = result.AddTime.ToString("yyyy-MM-dd"),
				AccountId = result.AccountId,
				Id = result.Id,
				IsResize = result.IsResize,
				IsOpenMax = result.IsOpenMax,
				IsFlash = result.IsFlash,
				AppStyle = (Int32)result.AppStyle,
				AppUrl = result.AppUrl,
				Width = result.Width,
				Height = result.Height,
				AppAuditState = (Int32)result.AppAuditState,
				AppReleaseState = (Int32)result.AppReleaseState,
				AppTypeId = result.AppTypeId,
				AccountName = result.AccountName,
				IsIconByUpload = result.IsIconByUpload
			};
		}

		public async Task<Boolean> IsInstallAppAsync(Int32 accountId, Int32 appId)
		{
			new Parameter().Validate(accountId).Validate(appId);
			var result = await _appContext.IsInstallAppAsync(accountId, appId);
			return result;
		}

		public IEnumerable<AppStyleDto> GetAppStyles()
		{
			var descriptions = GetEnumDescriptions(typeof(AppStyle));
			foreach (var description in descriptions)
			{
				yield return new AppStyleDto
				{
					Id = description.Id,
					Name = description.Value,
					Type = description.Type
				};
			}
		}

		public IEnumerable<AppStateDto> GetAppStates()
		{
			var appStates = new List<dynamic>();

			appStates.AddRange(GetEnumDescriptions(typeof(AppAuditState)));
			appStates.AddRange(GetEnumDescriptions(typeof(AppReleaseState)));
			foreach (var appState in appStates)
			{
				yield return new AppStateDto
				{
					Id = appState.Id,
					Name = appState.Value,
					Type = appState.Type
				};
			}
		}

		public async Task<List<AppDto>> GetSystemAppAsync(IEnumerable<Int32> appIds = default(IEnumerable<Int32>))
		{
			var result = await _appContext.GetSystemAppAsync(appIds);
			return result.Select(app => new AppDto
			{
				Id = app.Id,
				Name = app.Name,
				IconUrl = app.IconUrl
			}).ToList();
		}

		public async Task<Boolean> CheckAppTypeNameAsync(String appTypeName)
		{
			new Parameter().Validate(appTypeName);
			return await _appContext.CheckAppTypeNameAsync(appTypeName);
		}

		public async Task ModifyAppDirectionAsync(Int32 accountId, String direction)
		{
			new Parameter().Validate(accountId).Validate(direction);

			if (direction.ToLower() == "x")
			{
				await _deskContext.ModifyMemberDirectionToXAsync(accountId);
			}
			else if (direction.ToLower() == "y")
			{
				await _deskContext.ModifyMemberDirectionToYAsync(accountId);
			}
			else
			{
				throw new BusinessException($"未能识别的App排列方向:{direction.ToLower()}");
			}
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task ModifyAppIconSizeAsync(Int32 accountId, Int32 newSize)
		{
			new Parameter().Validate(accountId).Validate(newSize);
			await _deskContext.ModifyMemberDisplayIconSizeAsync(accountId, newSize);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task ModifyAppVerticalSpacingAsync(Int32 accountId, Int32 newSize)
		{
			new Parameter().Validate(accountId).Validate(newSize);
			await _deskContext.ModifyMemberHorizontalSpacingAsync(accountId, newSize);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task ModifyAppHorizontalSpacingAsync(Int32 accountId, Int32 newSize)
		{
			new Parameter().Validate(accountId).Validate(newSize);
			await _deskContext.ModifyMemberVerticalSpacingAsync(accountId, newSize);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		public async Task ModifyAppStarAsync(Int32 accountId, Int32 appId, Int32 starCount)
		{
			new Parameter().Validate(accountId).Validate(appId).Validate(starCount, true);

			var isInstall = await _appContext.IsInstallAppAsync(accountId, appId);
			if (!isInstall)
			{
				throw new BusinessException("您还没有安装这个App，因此不能打分");
			}
			await _appContext.ModifyAppStarAsync(accountId, appId, starCount);
		}

		public async Task InstallAppAsync(Int32 accountId, Int32 appId, Int32 deskNum)
		{
			new Parameter().Validate(accountId).Validate(appId).Validate(deskNum);
			await _appContext.InstallAsync(accountId, appId, deskNum);
			CacheHelper.RemoveOldKeyWhenModify(new DesktopCacheKey(accountId));
		}

		public async Task ModifyAccountAppInfoAsync(Int32 accountId, AppDto appDto)
		{
			new Parameter().Validate(accountId).Validate(appDto);
			await _appContext.ModifyAccountAppInfoAsync(accountId, appDto.ConvertToModel<AppDto, App>());
		}

		public async Task CreateNewAppAsync(AppDto appDto)
		{
			new Parameter().Validate(appDto);

			var app = appDto.ConvertToModel<AppDto, App>();
			var internalApp = new App(app.Name, app.IconUrl, app.AppUrl, app.Width, app.Height, app.AppTypeId, app.AppAuditState, AppReleaseState.UnRelease, app.AppStyle, app.AccountId,
				app.Remark, appDto.IsIconByUpload);

			await _appContext.CreateNewAppAsync(internalApp);
		}

		public async Task RemoveAppTypeAsync(Int32 appTypeId)
		{
			new Parameter().Validate(appTypeId);
			await _appContext.DeleteAppTypeAsync(appTypeId);
			CacheHelper.RemoveOldKeyWhenModify(new AppTypeCacheKey());
		}

		public async Task CreateNewAppTypeAsync(AppTypeDto appTypeDto)
		{
			new Parameter().Validate(appTypeDto);
			var appType = appTypeDto.ConvertToModel<AppTypeDto, AppType>();
			await _appContext.CreateNewAppTypeAsync(appType);
			CacheHelper.RemoveOldKeyWhenModify(new AppTypeCacheKey());
		}

		public async Task ModifyAppTypeAsync(AppTypeDto appTypeDto, Int32 appTypeId)
		{
			new Parameter().Validate(appTypeDto).Validate(appTypeId);
			await _appContext.ModifyAppTypeAsync(appTypeDto.Name, appTypeId);
			CacheHelper.RemoveOldKeyWhenModify(new AppTypeCacheKey());
		}

		public async Task PassAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await _appContext.PassAsync(appId);
		}

		public async Task DenyAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await _appContext.DenyAsync(appId);
		}

		public async Task SetTodayRecommandAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await _appContext.SetTodayRecommandAppAsync(appId);
		}

		public async Task RemoveAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await _appContext.RemoveAppAsync(appId);
		}

		public async Task ReleaseAppAsync(Int32 appId)
		{
			new Parameter().Validate(appId);
			await _appContext.ReleaseAppAsync(appId);
		}

		public async Task ModifyAppIconAsync(Int32 accountId, Int32 appId, String newIcon)
		{
			new Parameter().Validate(accountId).Validate(appId).Validate(newIcon);
			await _appContext.ModifyAppIconAsync(accountId, appId, newIcon);
		}

		#region private method
		// <summary>
		// <summary> 获取传入的枚举类型的字面量的描述
		private static IEnumerable<dynamic> GetEnumDescriptions(Type enumType)
		{
			return enumType.GetFields().Where(field => field.CustomAttributes.Any()).Select(s => new { s.CustomAttributes.ToArray()[0].ConstructorArguments[0].Value, Id = s.GetRawConstantValue(), Type = enumType.Name }).Cast<dynamic>().ToList();
		}

		#endregion
	}
}

