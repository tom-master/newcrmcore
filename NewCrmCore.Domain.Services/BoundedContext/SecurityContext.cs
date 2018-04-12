using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Infrastructure.CommonTools.CustomException;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCRM.Domain.Services.BoundedContext
{
	public class SecurityContext: ISecurityContext
	{
		public async Task<List<RolePower>> GetPowersAsync()
		{
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT a.RoleId, a.AppId FROM dbo.RolePower AS a WHERE a.IsDeleted=0";
					return dataStore.Find<RolePower>(sql);
				}
			});
		}

		public async Task<Role> GetRoleAsync(Int32 roleId)
		{
			new Parameter().Validate(roleId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT a.Name, a.RoleIdentity, a.Remark FROM dbo.Role AS a WHERE a.Id=@Id AND a.IsDeleted=0";
					var parameters = new List<SqlParameter> { new SqlParameter("@Id", roleId) };
					return dataStore.FindOne<Role>(sql, parameters);
				}
			});
		}

		public async Task<PagingModel<Role>> GetRolesAsync(String roleName, Int32 pageIndex, Int32 pageSize)
		{
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var paging = new PagingModel<Role>();

					var where = new StringBuilder();
					var parameters = new List<SqlParameter>();
					if (!String.IsNullOrEmpty(roleName))
					{
						parameters.Add(new SqlParameter("@roleName", $@"%{roleName}%"));
						where.Append($@" AND a.Name LIKE @roleName");
					}
					#region totalCount
					{
						var sql = $@"SELECT  COUNT(*) FROM dbo.Role AS a WHERE 1=1 {where} AND a.IsDeleted=0";
						paging.TotalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
					}
					#endregion

					#region sql
					{
						var sql = $@"SELECT TOP (@pageSize) * FROM 
                                (
	                                 SELECT
	                                ROW_NUMBER() OVER(ORDER BY a.Id DESC) AS rownumber,
	                                a.Name,
	                                a.RoleIdentity,
	                                a.Remark,
                                    a.Id
	                                FROM dbo.Role AS a WHERE 1=1 {where} AND a.IsDeleted=0
                                ) AS aa WHERE aa.rownumber>@pageSize*(@pageIndex-1)";
						parameters.Add(new SqlParameter("@pageIndex", pageIndex));
						parameters.Add(new SqlParameter("@pageSize", pageSize));
						paging.Models = dataStore.Find<Role>(sql, parameters);
					}
					#endregion

					return paging;
				}
			});
		}

		public async Task<Boolean> CheckPermissionsAsync(Int32 accessAppId, params Int32[] roleIds)
		{
			new Parameter().Validate(accessAppId).Validate(roleIds);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 检查app是否为系统app
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.App AS a WHERE a.Id=@Id AND a.IsDeleted=0 AND a.IsSystem=1";
						var parameters = new List<SqlParameter>
						{
							new SqlParameter("@Id",accessAppId)
						};
						var result = dataStore.FindSingleValue<Int32>(sql, parameters);
						if (result <= 0)
						{
							return true;
						}
					}
					#endregion

					{
						var sql = $@"SELECT a.AppId FROM dbo.RolePower AS a WHERE a.RoleId IN({String.Join(",", roleIds)}) AND a.IsDeleted=0";
						var result = dataStore.Find<RolePower>(sql);
						return result.Any(a => a.AppId == accessAppId);
					}
				}
			});
		}

		public async Task<Boolean> CheckRoleNameAsync(String name)
		{
			new Parameter().Validate(name);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT COUNT(*) FROM dbo.Role AS a WHERE a.Name=@name AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@name",name)
					};
					return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
				}
			});
		}

		public async Task<Boolean> CheckRoleIdentityAsync(String name)
		{
			new Parameter().Validate(name);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var sql = $@"SELECT COUNT(*) FROM dbo.Role AS a WHERE a.RoleIdentity=@RoleIdentity AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@RoleIdentity",name)
					};
					return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
				}
			});
		}

		public async Task ModifyRoleAsync(Role role)
		{
			new Parameter().Validate(role);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 修改角色
					{
						role.ModifyRoleName(role.Name);
						dataStore.ExecuteModify(role, r => r.Id == role.Id);
					}
					#endregion
				}
			});
		}

		public async Task RemoveRoleAsync(Int32 roleId)
		{
			new Parameter().Validate(roleId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					dataStore.OpenTransaction();
					try
					{
						var parameters = new List<SqlParameter>
						{
							new SqlParameter("@roleId",roleId)
						};
						#region 前置条件验证
						{
							var sql = $@"SELECT COUNT(*) FROM dbo.AccountRole AS a WHERE a.RoleId=@roleId AND a.IsDeleted=0";
							var result = dataStore.FindSingleValue<Int32>(sql, parameters);
							if (result > 0)
							{
								throw new BusinessException("当前角色已绑定了账户，无法删除");
							}
						}
						#endregion

						#region 删除角色
						{
							var role = new Role();
							role.Remove();
							dataStore.ExecuteModify(role, r => r.Id == roleId);
						}
						#endregion

						#region 移除权限
						{
							var rolePower = new RolePower();
							rolePower.Remove();
							dataStore.ExecuteModify(rolePower, rp => rp.RoleId == roleId);
						}
						#endregion

						dataStore.Commit();
					}
					catch (Exception)
					{
						dataStore.Rollback();
						throw;
					}
				}
			});
		}

		public async Task AddNewRoleAsync(Role role)
		{
			new Parameter().Validate(role);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					#region 添加角色
					{
						dataStore.ExecuteAdd(role);
					}
					#endregion
				}
			});
		}

		public async Task AddPowerToCurrentRoleAsync(Int32 roleId, IEnumerable<Int32> powerIds)
		{
			new Parameter().Validate(roleId).Validate(powerIds);
			await Task.Run(() =>
			{
				if (!powerIds.Any())
				{
					throw new BusinessException("权限列表为空");
				}

				using (var dataStore = new DataStore())
				{
					dataStore.OpenTransaction();
					try
					{
						#region 移除之前的角色权限
						{
							var rolePower = new RolePower();
							rolePower.Remove();
							dataStore.ExecuteModify(rolePower, rp => rp.RoleId == roleId);
						}
						#endregion

						#region 添加角色权限
						{
							var sqlBuilder = new StringBuilder();
							foreach (var item in powerIds)
							{
								var rolePower = new RolePower(roleId, item);
								dataStore.ExecuteAdd(rolePower);
							}
						}
						#endregion

						dataStore.Commit();
					}
					catch (Exception)
					{
						dataStore.Rollback();
						throw;
					}
				}
			});
		}
	}
}
