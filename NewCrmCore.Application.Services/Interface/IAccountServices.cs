using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
    public interface IUserServices
    {
        #region  have return value

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserDto> LoginAsync(String userName, String password, String requestIp);

        /// <summary>
        /// 获取登陆用户的配置文件
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ConfigDto> GetConfigAsync(Int32 userId);

        /// <summary>
        /// 获取全部的用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        Task<PageList<UserDto>> GetUsersAsync(String userName, String userType, Int32 pageIndex, Int32 pageSize);

        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserDto> GetUserAsync(Int32 userId = default(Int32));

        /// <summary>
        /// 验证相同的用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Boolean> CheckUserNameExistAsync(String userName);

        /// <summary>
        ///  检查密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldUserPassword"></param>
        /// <returns></returns>
        Task<Boolean> CheckPasswordAsync(Int32 userId, String oldUserPassword);

        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="unlockPassword"></param>
        /// <returns></returns>
        Task<Boolean> UnlockScreenAsync(Int32 userId, String unlockPassword);

        /// <summary>
        /// 检查app名称是否已经存在
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

        #endregion

        #region not have return value

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
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task ModifyUserAsync(UserDto user);

        /// <summary>
        /// 修改账户头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newFace"></param>
        Task ModifyUserFaceAsync(Int32 userId, String newFace);

        /// <summary>
        /// 添加新的用户
        /// </summary>
        /// <param name="user"></param>
        Task AddNewUserAsync(UserDto user);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task LogoutAsync(Int32 userId);

        /// <summary>
        /// 用户启用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task EnableAsync(Int32 userId);

        /// <summary>
        /// 用户禁用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DisableAsync(Int32 userId);

        /// <summary>
        /// 移除账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveUserAsync(Int32 userId);

        #endregion
    }
}
