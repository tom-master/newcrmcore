using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
	public interface ISecurityServices
	{
		#region have return value

		/// <summary>
		/// 获取全部的角色
		/// </summary>
		Task<PagingModel<RoleDto>> GetRolesAsync(String roleName, Int32 pageIndex, Int32 pageSize);

		/// <summary>
		/// 根据角色Id获取角色
		/// </summary>
		Task<RoleDto> GetRoleAsync(Int32 roleId);

		/// <summary>
		/// 检查用户权限
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

		#endregion

		#region not have return value

		/// <summary>
		/// 新建角色
		/// </summary>
		Task AddNewRoleAsync(RoleDto role);

		/// <summary>
		/// 修改角色信息
		/// </summary>
		Task ModifyRoleAsync(RoleDto role);

		/// <summary>
		/// 添加权限到当前的角色
		/// </summary>
		Task AddPowerToCurrentRoleAsync(Int32 roleId, IEnumerable<Int32> powerIds);

		/// <summary>
		/// 移除角色
		/// </summary>
		Task RemoveRoleAsync(Int32 roleId);

		#endregion
	}
}
