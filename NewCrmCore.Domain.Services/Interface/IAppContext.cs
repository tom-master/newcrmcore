using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
	public interface IAppContext
	{
		/// <summary>
		/// 获取所有的app
		/// </summary>
		List<App> GetApps(Int32 accountId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

		/// <summary>
		/// 获取当前账户下所有的app
		/// </summary>
		List<App> GetAccountApps(Int32 accountId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

		/// <summary>
		/// 获取app
		/// </summary>
		Task<App> GetAppAsync(Int32 appId);

		/// <summary>
		/// 是否已经安装app
		/// </summary>
		Task<Boolean> IsInstallAppAsync(Int32 accountId, Int32 appId);

		/// <summary>
		/// 获取系统App
		/// </summary>
		Task<List<App>> GetSystemAppAsync(IEnumerable<Int32> appIds = default(IEnumerable<Int32>));


		/// <summary>
		/// 获取当前账户下已开发和未发布的app
		/// </summary>
		Task<Tuple<Int32, Int32>> GetAccountDevelopAppCountAndNotReleaseAppCountAsync(Int32 accountId);

		/// <summary>
		/// 获取所有App类型
		/// </summary>
		/// <returns></returns>
		Task<List<AppType>> GetAppTypesAsync();

		/// <summary>
		/// 获取今日推荐
		/// </summary>
		Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 accountId);

		/// <summary>
		/// 检查应用类型名称
		/// </summary>
		Task<Boolean> CheckAppTypeNameAsync(String appTypeName);

		/// <summary>
		/// 更改app评分
		/// </summary>
		Task ModifyAppStarAsync(Int32 accountId, Int32 appId, Int32 starCount);

		/// <summary>
		/// 创建新的app
		/// </summary>
		Task CreateNewAppAsync(App app);

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
		/// 发布app
		/// </summary>
		Task ReleaseAppAsync(Int32 appId);

		/// <summary>
		/// 修改开发者（用户）的app信息
		/// </summary>
		Task ModifyAccountAppInfoAsync(Int32 accountId, App app);

		/// <summary>
		/// 删除App分类
		/// </summary>
		Task DeleteAppTypeAsync(Int32 appTypeId);

		/// <summary>
		/// 创建新的app分类
		/// </summary>
		Task CreateNewAppTypeAsync(AppType appType);

		/// <summary>
		/// 修改app分类
		/// </summary>
		Task ModifyAppTypeAsync(String appTypeName, Int32 appTypeId);

		/// <summary>
		/// 更改app图标
		/// </summary>
		Task ModifyAppIconAsync(Int32 accountId, Int32 appId, String newIcon);

		/// <summary>
		/// 用户安装app
		/// </summary> 
		Task InstallAsync(Int32 accountId, Int32 appId, Int32 deskNum);
	}
}
