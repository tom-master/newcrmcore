using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
	public interface IAccountContext
	{
		/// <summary>
		/// 验证用户是否合法
		/// </summary>
		Task<Account> ValidateAsync(String accountName, String password, String requestIp);

		/// <summary>
		/// 获取配置
		/// </summary>
		Task<Config> GetConfigAsync(Int32 accountId);

		/// <summary>
		/// 获取壁纸
		/// </summary>
		Task<Wallpaper> GetWallpaperAsync(Int32 wallPaperId);

		/// <summary>
		/// 获取所有账户
		/// </summary>
		Task<PagingModel<Account>> GetAccountsAsync(String accountName, String accountType, Int32 pageIndex, Int32 pageSize);

		/// <summary>
		/// 获取单个账户
		/// </summary>
		Task<Account> GetAccountAsync(Int32 accountId);

		/// <summary>
		/// 获取账户权限
		/// </summary>
		Task<List<Role>> GetRolesAsync(Int32 accountId);

		/// <summary>
		/// 获取角色所属权限
		/// </summary>
		Task<List<RolePower>> GetPowersAsync();

		/// <summary>
		/// 检查用户名是否重复
		/// </summary>
		Task<Boolean> CheckAccountNameExistAsync(String accountName);

		/// <summary>
		/// 检查密码
		/// </summary>
		Task<String> GetOldPasswordAsync(Int32 accountId);

		/// <summary>
		/// 解除屏幕锁定
		/// </summary>
		Task<Boolean> UnlockScreenAsync(Int32 accountId, String unlockPassword);

		/// <summary>
		/// 检查app名称是否已经存在
		/// </summary>
		Task<Boolean> CheckAppNameAsync(String name);

		/// <summary>
		/// 检查appUrl是否存在
		/// </summary>
		Task<Boolean> CheckAppUrlAsync(String url);

		/// <summary>
		/// 添加新账户
		/// </summary>
		Task AddNewAccountAsync(Account accountDto);

		/// <summary>
		/// 修改账户
		/// </summary>
		Task ModifyAccountAsync(Account accountDto);

		/// <summary>
		/// 账户启用
		/// </summary>
		Task EnableAsync(Int32 accountId);

		/// <summary>
		/// 账户禁用
		/// </summary>
		Task DisableAsync(Int32 accountId);

		/// <summary>
		/// 修改账户头像
		/// </summary>
		Task ModifyAccountFaceAsync(Int32 accountId, String newFace);

		/// <summary>
		/// 修改账户密码
		/// </summary>
		Task ModifyPasswordAsync(Int32 accountId, String newPassword, Boolean isTogetherSetLockPassword);

		/// <summary>
		/// 修改锁屏密码
		/// </summary>
		Task ModifyLockScreenPasswordAsync(Int32 accountId, String newScreenPassword);

		/// <summary>
		/// 移除账户
		/// </summary>
		Task RemoveAccountAsync(Int32 accountId);

		/// <summary>
		/// 用户登出
		/// </summary>
		Task LogoutAsync(Int32 accountId);
	}
}
