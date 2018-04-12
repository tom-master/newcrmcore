using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
	public interface ISecurityContext
	{
		/// <summary>
		/// 获取角色
		/// </summary>
		Task<Role> GetRoleAsync(Int32 roleId);

		/// <summary>
		/// 获取权限列表
		/// </summary>
		/// <returns></returns>
		Task<List<RolePower>> GetPowersAsync();

		/// <summary>
		/// 获取角色列表
		/// </summary>
		Task<PagingModel<Role>> GetRolesAsync(String roleName, Int32 pageIndex, Int32 pageSize);

		/// <summary>
		/// 检查授权
		/// </summary>
		Task<Boolean> CheckPermissionsAsync(Int32 accessAppId, params Int32[] roleIds);

		/// <summary>
		/// 检查角色名称
		/// </summary>
		Task<Boolean> CheckRoleNameAsync(String name);

		/// <summary>
		/// 检查角色标识
		/// </summary>
		Task<Boolean> CheckRoleIdentityAsync(String name);

		/// <summary>
		/// 移除角色
		/// </summary>
		Task RemoveRoleAsync(Int32 roleId);

		/// <summary>
		/// 添加新角色
		/// </summary>
		Task AddNewRoleAsync(Role role);

		/// <summary>
		/// 修改角色 
		/// </summary>
		Task ModifyRoleAsync(Role role);

		/// <summary>
		/// 添加权限到角色
		/// </summary>
		Task AddPowerToCurrentRoleAsync(Int32 roleId, IEnumerable<Int32> powerIds);
	}
}
