using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class SecurityContext : ISecurityContext
    {
        public async Task<List<RolePower>> GetPowersAsync()
        {
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.RoleId, a.AppId FROM RolePower AS a WHERE a.IsDeleted=0";
                    return dataStore.Find<RolePower>(sql);
                }
            });
        }

        public async Task<Role> GetRoleAsync(Int32 roleId)
        {
            Parameter.Validate(roleId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.Id, a.Name, a.RoleIdentity, a.Remark FROM Role AS a WHERE a.Id=@Id AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@Id", roleId) };
                    return dataStore.FindOne<Role>(sql, parameters);
                }
            });
        }

        public List<Role> GetRoles(String roleName, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            using (var dataStore = new DataStore(Appsetting.Database))
            {
                var where = new StringBuilder();
                var parameters = new List<ParameterMapper>();
                if (!String.IsNullOrEmpty(roleName))
                {
                    parameters.Add(new ParameterMapper("@roleName", $@"%{roleName}%"));
                    where.Append($@" AND a.Name LIKE @roleName");
                }

                #region totalCount
                {
                    var sql = $@"SELECT  COUNT(*) FROM Role AS a WHERE 1=1 {where} AND a.IsDeleted=0";
                    totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
                }
                #endregion

                #region sql
                {
                    var sql = $@"SELECT
								a.Name,
								a.RoleIdentity,
								a.Remark,
								a.Id
								FROM Role AS a WHERE 1=1 {where} AND a.IsDeleted=0 LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    return dataStore.Find<Role>(sql, parameters);
                }
                #endregion

            }
        }

        public async Task<Boolean> CheckPermissionsAsync(Int32 accessAppId, params Int32[] roleIds)
        {
            Parameter.Validate(accessAppId);
            Parameter.Validate(roleIds);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 检查app是否为系统app
                    {
                        var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.Id=@Id AND a.IsDeleted=0 AND a.IsSystem=1";
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@Id",accessAppId)
                        };
                        var result = dataStore.FindSingleValue<Int32>(sql, parameters);
                        if (result <= 0)
                        {
                            return true;
                        }
                    }
                    #endregion

                    {
                        var sql = $@"SELECT a.AppId FROM RolePower AS a WHERE a.RoleId IN({String.Join(",", roleIds)}) AND a.IsDeleted=0";
                        var result = dataStore.Find<RolePower>(sql);
                        return result.Any(a => a.AppId == accessAppId);
                    }
                }
            });
        }

        public async Task<Boolean> CheckRoleNameAsync(String name)
        {
            Parameter.Validate(name);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM Role AS a WHERE a.Name=@name AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@name",name)
                    };
                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }

        public async Task<Boolean> CheckRoleIdentityAsync(String name)
        {
            Parameter.Validate(name);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM Role AS a WHERE a.RoleIdentity=@RoleIdentity AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@RoleIdentity",name)
                    };
                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }

        public async Task ModifyRoleAsync(Role role)
        {
            Parameter.Validate(role);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 修改角色
                    {
                        role.ModifyRoleName(role.Name);
                        dataStore.Modify(role, r => r.Id == role.Id);
                    }
                    #endregion
                }
            });
        }

        public async Task RemoveRoleAsync(Int32 roleId)
        {
            Parameter.Validate(roleId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@roleId",roleId)
                        };
                        #region 前置条件验证
                        {
                            var sql = $@"SELECT COUNT(*) FROM AccountRole AS a WHERE a.RoleId=@roleId AND a.IsDeleted=0";
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
                            dataStore.Modify(role, r => r.Id == roleId);
                        }
                        #endregion

                        #region 移除权限
                        {
                            var rolePower = new RolePower();
                            rolePower.Remove();
                            dataStore.Modify(rolePower, rp => rp.RoleId == roleId);
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
            Parameter.Validate(role);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 添加角色
                    {
                        dataStore.Add(role);
                    }
                    #endregion
                }
            });
        }

        public async Task AddPowerToCurrentRoleAsync(Int32 roleId, IEnumerable<Int32> powerIds)
        {
            Parameter.Validate(roleId);
            Parameter.Validate(powerIds);
            await Task.Run(() =>
            {
                if (!powerIds.Any())
                {
                    throw new BusinessException("权限列表为空");
                }

                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        #region 移除之前的角色权限
                        {
                            var rolePower = new RolePower();
                            rolePower.Remove();
                            dataStore.Modify(rolePower, rp => rp.RoleId == roleId);
                        }
                        #endregion

                        #region 添加角色权限
                        {
                            var sqlBuilder = new StringBuilder();
                            foreach (var item in powerIds)
                            {
                                var rolePower = new RolePower(roleId, item);
                                dataStore.Add(rolePower);
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
