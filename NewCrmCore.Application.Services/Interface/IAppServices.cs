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
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<TodayRecommendAppDto> GetTodayRecommendAsync(Int32 userId);

        /// <summary>
        /// 获取用户开发的应用和未发布的应用的数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(Int32 allCount, Int32 notReleaseCount)> GetDevelopAndNotReleaseCountAsync(Int32 userId);

        /// <summary>
        /// 获取所有的应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appTypeId"></param>
        /// <param name="orderId"></param>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageList<AppDto>> GetAppsAsync(Int32 userId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize);

        /// <summary>
        /// 获取用户的应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchText"></param>
        /// <param name="appTypeId"></param>
        /// <param name="appStyleId"></param>
        /// <param name="appState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageList<AppDto>> GetUserAppsAsync(Int32 userId, String searchText, Int32 appTypeId, Int32 appStyleId, String appState, Int32 pageIndex, Int32 pageSize);

        /// <summary>
        /// 根据appId获取应用
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<AppDto> GetAppAsync(Int32 appId, Int32 userId);

        /// <summary>
        /// 当前用户是否安装了这个应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<Boolean> IsInstallAppAsync(Int32 userId, Int32 appId);

        /// <summary>
        /// 获取所有的应用样式
        /// </summary>
        /// <returns></returns>
        IEnumerable<AppStyleDto> GetAppStyles();

        /// <summary>
        /// 获取所有的应用状态
        /// </summary>
        /// <returns></returns>
        IEnumerable<AppStateDto> GetAppStates();

        /// <summary>
        /// 获取系统应用
        /// </summary>
        /// <param name="appIds"></param>
        /// <returns></returns>
        Task<List<AppDto>> GetSystemAppAsync(IEnumerable<Int32> appIds = default);

        /// <summary>
        /// 检查应用类型
        /// </summary>
        /// <param name="appTypeName"></param>
        /// <returns></returns>
        Task<Boolean> CheckAppTypeNameAsync(String appTypeName);

        #endregion

        #region not have return value

        /// <summary>
        /// 修改开发者（用户）的应用信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appDto"></param>
        /// <returns></returns>
        Task ModifyUserAppInfoAsync(Int32 userId, AppDto appDto);

        /// <summary>
        /// 开发者（用户）创建新的应用
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        Task CreateNewAppAsync(AppDto app);

        /// <summary>
        /// 删除指定的应用类型
        /// </summary>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        Task RemoveAppTypeAsync(Int32 appTypeId);

        /// <summary>
        /// 创建新的应用类型
        /// </summary>
        /// <param name="appTypeDto"></param>
        /// <returns></returns>
        Task CreateNewAppTypeAsync(AppTypeDto appTypeDto);

        /// <summary>
        /// 修改应用类型
        /// </summary>
        /// <param name="appTypeDto"></param>
        /// <param name="appTypeId"></param>
        /// <returns></returns>
        Task ModifyAppTypeAsync(AppTypeDto appTypeDto, Int32 appTypeId);

        /// <summary>
        /// 应用审核通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task PassAsync(Int32 appId);

        /// <summary>
        /// 应用审核不通过
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task DenyAsync(Int32 appId);

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
        Task ReleaseAppAsync(Int32 appId);

        /// <summary>
        /// 应用打分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="starCount"></param>
        /// <returns></returns>
        Task ModifyAppStarAsync(Int32 userId, Int32 appId, Int32 starCount);

        /// <summary>
        /// 安装应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="deskNum"></param>
        /// <returns></returns>
        Task InstallAppAsync(Int32 userId, Int32 appId, Int32 deskNum);

        /// <summary>
        /// 修改应用排列方向
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        Task ModifyAppDirectionAsync(Int32 userId, String direction);

        /// <summary>
        /// 修改应用图标大小
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        Task ModifyAppIconSizeAsync(Int32 userId, Int32 newSize);

        /// <summary>
        /// 修改应用垂直间距
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        Task ModifyAppVerticalSpacingAsync(Int32 userId, Int32 newSize);

        /// <summary>
        /// 修改应用水平间距
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        Task ModifyAppHorizontalSpacingAsync(Int32 userId, Int32 newSize);

        /// <summary>
        /// 修改应用图标
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="newIcon"></param>
        /// <returns></returns>
        Task ModifyAppIconAsync(Int32 userId, Int32 appId, String newIcon);

        #endregion
    }
}
