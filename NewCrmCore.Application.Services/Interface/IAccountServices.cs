using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
    public interface IAccountServices
    {
        #region  have return value

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<AccountDto> LoginAsync(String accountName, String password, String requestIp);

        /// <summary>
        /// 获取登陆用户的配置文件
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<ConfigDto> GetConfigAsync(Int32 accountId);

        /// <summary>
        /// 获取全部的用户
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="accountType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        Task<PageList<AccountDto>> GetAccountsAsync(String accountName, String accountType, Int32 pageIndex, Int32 pageSize);

        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<AccountDto> GetAccountAsync(Int32 accountId = default(Int32));

        /// <summary>
        /// 验证相同的用户名是否存在
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        Task<Boolean> CheckAccountNameExistAsync(String accountName);

        /// <summary>
        ///  检查密码
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="oldAccountPassword"></param>
        /// <returns></returns>
        Task<Boolean> CheckPasswordAsync(Int32 accountId, String oldAccountPassword);

        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="unlockPassword"></param>
        /// <returns></returns>
        Task<Boolean> UnlockScreenAsync(Int32 accountId, String unlockPassword);

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
        /// <param name="accountId"></param>
        /// <param name="newPassword"></param>
        /// <param name="isTogetherSetLockPassword"></param>
        /// <returns></returns>
        Task ModifyPasswordAsync(Int32 accountId, String newPassword, Boolean isTogetherSetLockPassword);

        /// <summary>
        /// 修改锁屏密码
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newScreenPassword"></param>
        /// <returns></returns>
        Task ModifyLockScreenPasswordAsync(Int32 accountId, String newScreenPassword);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task ModifyAccountAsync(AccountDto account);

        /// <summary>
        /// 修改账户头像
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newFace"></param>
        Task ModifyAccountFaceAsync(Int32 accountId, String newFace);

        /// <summary>
        /// 添加新的用户
        /// </summary>
        /// <param name="account"></param>
        Task AddNewAccountAsync(AccountDto account);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task LogoutAsync(Int32 accountId);

        /// <summary>
        /// 用户启用
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task EnableAsync(Int32 accountId);

        /// <summary>
        /// 用户禁用
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task DisableAsync(Int32 accountId);

        /// <summary>
        /// 移除账户
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task RemoveAccountAsync(Int32 accountId);

        #endregion
    }
}
