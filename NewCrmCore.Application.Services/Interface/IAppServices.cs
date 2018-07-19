using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
	public interface IAppServices
	{
		#region have return value

		/// <summary>
		/// 获取所有的app类型
		/// </summary>
		/// <returns></returns>
		Task<List<AppTypeDto>> GetAppTypesAsync();

		/// <summary>
		/// 获取今日推荐
		/// </summary>
		/// <returns></returns>
		Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 accountId);

		/// <summary>
		/// 获取用户开发的app和未发布的app
		/// </summary>
		/// <returns></returns>
		Task<Tuple<Int32, Int32>> GetDevelopAndNotReleaseCountAsync(Int32 accountId);

		/// <summary>
		/// 获取所有的app
		/// </summary>
		/// <returns></returns>
		Task<PageList<AppDto>> GetAppsAsync(Int32 accountId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize);

		/// <summary>
		/// 获取用户的app
		/// </summary>
		/// <returns></returns>
		Task<PageList<AppDto>> GetAccountAppsAsync(Int32 accountId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize);

		/// <summary>
		/// 根据appId获取App
		/// </summary>
		/// <returns></returns>
		Task<AppDto> GetAppAsync(Int32 appId);

		/// <summary>
		/// 当前用户是否安装了这个app
		/// </summary>
		/// <returns></returns>
		Task<Boolean> IsInstallAppAsync(Int32 accountId, Int32 appId);

		/// <summary>
		/// 获取所有的app样式
		/// </summary>
		/// <returns></returns>
		IEnumerable<AppStyleDto> GetAppStyles();

		/// <summary>
		/// 获取所有的app状态
		/// </summary>
		/// <returns></returns>
		IEnumerable<AppStateDto> GetAppStates();

		/// <summary>
		/// 获取系统app
		/// </summary>
		/// <returns></returns>
		Task<List<AppDto>> GetSystemAppAsync(IEnumerable<Int32> appIds = default(IEnumerable<Int32>));

		/// <summary>
		/// 检查应用类型
		/// </summary>
		Task<Boolean> CheckAppTypeNameAsync(String appTypeName);

		#endregion

		#region not have return value

		/// <summary>
		/// 修改开发者（用户）的app信息
		/// </summary>
		Task ModifyAccountAppInfoAsync(Int32 accountId, AppDto appDto);

		/// <summary>
		/// 开发者（用户）创建新的app
		/// </summary>
		Task CreateNewAppAsync(AppDto app);

		/// <summary>
		/// 删除指定的应用类型
		/// </summary>
		Task RemoveAppTypeAsync(Int32 appTypeId);

		/// <summary>
		/// 创建新的app类型
		/// </summary>
		Task CreateNewAppTypeAsync(AppTypeDto appTypeDto);

		/// <summary>
		/// 修改app类型
		/// </summary>
		Task ModifyAppTypeAsync(AppTypeDto appTypeDto, Int32 appTypeId);

		/// <summary>
		/// app审核通过
		/// </summary>
		Task PassAsync(Int32 appId);

		/// <summary>
		/// app审核不通过
		/// </summary>
		Task DenyAsync(Int32 appId);

		/// <summary>
		/// 设置今日推荐app
		/// </summary>
		Task SetTodayRecommandAppAsync(Int32 appId);

		/// <summary>
		/// 移除app
		/// </summary>
		Task RemoveAppAsync(Int32 appId);

		/// <summary>
		/// 发布应用
		/// </summary>
		Task ReleaseAppAsync(Int32 appId);

		/// <summary>
		/// app打分
		/// </summary>
		Task ModifyAppStarAsync(Int32 accountId, Int32 appId, Int32 starCount);

		/// <summary>
		/// 安装app
		/// </summary>
		Task InstallAppAsync(Int32 accountId, Int32 appId, Int32 deskNum);

		/// <summary>
		/// 修改app排列方向
		/// </summary>
		Task ModifyAppDirectionAsync(Int32 accountId, String direction);

		/// <summary>
		/// 修改app图标大小
		/// </summary>
		Task ModifyAppIconSizeAsync(Int32 accountId, Int32 newSize);

		/// <summary>
		/// 修改app垂直间距
		/// </summary>
		Task ModifyAppVerticalSpacingAsync(Int32 accountId, Int32 newSize);

		/// <summary>
		/// 修改app水平间距
		/// </summary>
		Task ModifyAppHorizontalSpacingAsync(Int32 accountId, Int32 newSize);

		/// <summary>
		/// 修改app图标
		/// </summary>
		Task ModifyAppIconAsync(Int32 accountId, Int32 appId, String newIcon);

		#endregion
	}
}
