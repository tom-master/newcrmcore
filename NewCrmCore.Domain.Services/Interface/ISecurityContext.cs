using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
    public interface ISecurityContext
    {
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<Role> GetRoleAsync(Int32 roleId);

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        Task<List<RolePower>> GetPowersAsync();

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Role> GetRoles(String roleName, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 检查授权
        /// </summary>
        /// <param name="accessAppId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<Boolean> CheckPermissionsAsync(Int32 accessAppId, params Int32[] roleIds);

        /// <summary>
        /// 检查角色名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Boolean> CheckRoleNameAsync(String name);

        /// <summary>
        /// 检查角色标识
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Boolean> CheckRoleIdentityAsync(String name);

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task RemoveRoleAsync(Int32 roleId);

        /// <summary>
        /// 添加新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task AddNewRoleAsync(Role role);

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task ModifyRoleAsync(Role role);

        /// <summary>
        /// 添加权限到角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="powerIds"></param>
        /// <returns></returns>
        Task AddPowerToCurrentRoleAsync(Int32 roleId, IEnumerable<Int32> powerIds);

        /// <summary>
        /// 添加访客记录
        /// </summary>
        /// <param name="visitorRecord"></param>
        /// <returns></returns>
        Task AddVisitorRecord(VisitorRecord visitorRecord);
    }
}
