using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
    public interface IUserContext
    {
        /// <summary>
        /// 验证用户是否合法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="requestIp"></param>
        /// <returns></returns>
        Task<User> IfNullOrZeroAsync(String userName, String password, String requestIp);

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Config> GetConfigAsync(Int32 userId);

        /// <summary>
        /// 获取壁纸
        /// </summary>
        /// <param name="wallPaperId"></param>
        /// <returns></returns>
        Task<Wallpaper> GetWallpaperAsync(Int32 wallPaperId);

        /// <summary>
        /// 获取所有账户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<User> GetUsers(String userName, String userType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 获取单个账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> GetUserAsync(Int32 userId);

        /// <summary>
        /// 获取账户权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Role>> GetRolesAsync(Int32 userId);

        /// <summary>
        /// 获取角色所属权限
        /// </summary>
        /// <returns></returns>
        Task<List<RolePower>> GetPowersAsync();

        /// <summary>
        /// 检查用户名是否重复
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Boolean> CheckUserNameExistAsync(String userName);

        /// <summary>
        /// 检查密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<String> GetOldPasswordAsync(Int32 userId);

        /// <summary>
        /// 解除屏幕锁定
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="unlockPassword"></param>
        /// <returns></returns>
        Task<Boolean> UnlockScreenAsync(Int32 userId, String unlockPassword);

        /// <summary>
        /// 检查应用名称是否已经存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Boolean> CheckAppNameAsync(String name);

        /// <summary>
        /// 检查appUrl是否存在
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Boolean> CheckAppUrlAsync(String url);

        /// <summary>
        /// 添加新账户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        Task AddNewUserAsync(User userDto);

        /// <summary>
        /// 修改账户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        Task ModifyUserAsync(User userDto);

        /// <summary>
        /// 账户启用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task EnableAsync(Int32 userId);

        /// <summary>
        /// 账户禁用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DisableAsync(Int32 userId);

        /// <summary>
        /// 修改账户头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newFace"></param>
        /// <returns></returns>
        Task ModifyUserFaceAsync(Int32 userId, String newFace);

        /// <summary>
        /// 修改账户密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="isTogetherSetLockPassword"></param>
        /// <returns></returns>
        Task ModifyPasswordAsync(Int32 userId, String newPassword, Boolean isTogetherSetLockPassword);

        /// <summary>
        /// 修改锁屏密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newScreenPassword"></param>
        /// <returns></returns>
        Task ModifyLockScreenPasswordAsync(Int32 userId, String newScreenPassword);

        /// <summary>
        /// 移除账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveUserAsync(Int32 userId);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task LogoutAsync(Int32 userId);
    }
}
