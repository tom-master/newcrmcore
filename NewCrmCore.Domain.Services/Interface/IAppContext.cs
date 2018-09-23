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
        /// 获取所有的应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appTypeId"></param>
        /// <param name="orderId"></param>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<App> GetApps(Int32 userId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 获取当前账户下所有的应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchText"></param>
        /// <param name="appTypeId"></param>
        /// <param name="appStyleId"></param>
        /// <param name="appState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<App> GetUserApps(Int32 userId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 获取应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<App> GetAppAsync(Int32 appId);

        /// <summary>
        /// 是否已经安装应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<Boolean> IsInstallAppAsync(Int32 userId, Int32 appId);

        /// <summary>
        /// 获取系统应用
        /// </summary>
        /// <param name="appIds"></param>
        /// <returns></returns>
        Task<List<App>> GetSystemAppAsync(IEnumerable<Int32> appIds = default(IEnumerable<Int32>));

        /// <summary>
        /// 获取当前账户下已开发和未发布的应用数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Tuple<Int32, Int32>> GetDevelopAndNotReleaseCountAsync(Int32 userId);

        /// <summary>
        /// 获取所有应用类型
        /// </summary>
        /// <returns></returns>
        Task<List<AppType>> GetAppTypesAsync();

        /// <summary>
        /// 获取今日推荐
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 userId);

        /// <summary>
        /// 检查应用类型名称
        /// </summary>
        /// <param name="appTypeName"></param>
        /// <returns></returns>
        Task<Boolean> CheckAppTypeNameAsync(String appTypeName);

        /// <summary>
        /// 更改应用评分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="starCount"></param>
        /// <returns></returns>
        Task ModifyAppStarAsync(Int32 userId, Int32 appId, Int32 starCount);

        /// <summary>
        /// 创建新的应用
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        Task CreateNewAppAsync(App app);

        /// <summary>
        /// 应用审核通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<App> PassAsync(Int32 appId);

        /// <summary>
        /// 应用审核不通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<App> DenyAsync(Int32 appId);

        /// <summary>
        /// 设置今日推荐应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task SetTodayRecommandAppAsync(Int32 appId);

        /// <summary>
        /// 移除应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task RemoveAppAsync(Int32 appId);

        /// <summary>
        /// 发布应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<App> ReleaseAppAsync(Int32 appId);

        /// <summary>
        /// 修改开发者（用户）的应用信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        Task ModifyUserAppInfoAsync(Int32 userId, App app);

        /// <summary>
        /// 删除应用分类
        /// </summary>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        Task DeleteAppTypeAsync(Int32 appTypeId);

        /// <summary>
        /// 创建新的应用分类
        /// </summary>
        /// <param name="appType"></param>
        /// <returns></returns>
        Task CreateNewAppTypeAsync(AppType appType);

        /// <summary>
        /// 修改应用分类
        /// </summary>
        /// <param name="appTypeName"></param>
        /// <param name="isSystem"></param>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        Task ModifyAppTypeAsync(String appTypeName,Boolean isSystem, Int32 appTypeId);

        /// <summary>
        /// 更改应用图标
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="newIcon"></param>
        /// <returns></returns>
        Task ModifyAppIconAsync(Int32 userId, Int32 appId, String newIcon);

        /// <summary>
        /// 用户安装应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="deskNum"></param>
        /// <returns></returns>
        Task<App> InstallAsync(Int32 userId, Int32 appId, Int32 deskNum);
    }
}
