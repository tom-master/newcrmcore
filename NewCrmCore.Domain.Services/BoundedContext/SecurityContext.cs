using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class SecurityContext : ISecurityContext
    {
        public async Task<List<RolePower>> GetPowersAsync()
        {
            return await Task.Run(() =>
            {
                using (var mapper = new EntityMapper())
                {
                    return mapper.Find<RolePower>(a => new
                    {
                        a.RoleId,
                        a.AppId
                    }).ToList();

                    //var sql = $@"SELECT a.RoleId, a.AppId FROM RolePower AS a WHERE a.IsDeleted=0";
                    //return mapper.Find<RolePower>(sql);
                }
            });
        }

        public async Task<Role> GetRoleAsync(Int32 roleId)
        {
            Parameter.Validate(roleId);

            return await Task.Run(() =>
            {
                using (var mapper = new EntityMapper())
                {
                    return mapper.Find<Role>(a => a.Id == roleId, a => new { a.Id, a.Name, a.RoleIdentity }).FirstOrDefault();
                    //var sql = $@"SELECT a.Id, a.Name, a.RoleIdentity, a.Remark FROM Role AS a WHERE a.Id=@Id AND a.IsDeleted=0";
                    //var parameters = new List<EntityParameter> { new EntityParameter("@Id", roleId) };
                    //return mapper.FindOne<Role>(sql, parameters);
                }
            });
        }

        public List<Role> GetRoles(String roleName, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            using (var mapper = new EntityMapper())
            {
                var where = new StringBuilder();
                var parameters = new List<EntityParameter>();
                if (!String.IsNullOrEmpty(roleName))
                {
                    parameters.Add(new EntityParameter("@roleName", $@"%{roleName}%"));
                    where.Append($@" AND a.Name LIKE @roleName");
                }

                #region totalCount
                {
                    var sql = $@"SELECT  COUNT(*) FROM Role AS a WHERE 1=1 {where} AND a.IsDeleted=0";
                    totalCount = mapper.ComplexSqlExecute<Int32>(sql, parameters);
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
                    return mapper.ComplexSqlExecute<List<Role>>(sql, parameters);
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
                using (var mapper = new EntityMapper())
                {
                    #region 检查app是否为系统app
                    {

                        var result = mapper.Count<App>(a => a.Id == accessAppId && a.IsSystem);
                        //var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.Id=@Id AND a.IsDeleted=0 AND a.IsSystem=1";
                        //var parameters = new List<EntityParameter>
                        //{
                        //    new EntityParameter("@Id",accessAppId)
                        //};
                        //var result = mapper.FindSingleValue<Int32>(sql, parameters);
                        if (result <= 0)
                        {
                            return true;
                        }
                    }
                    #endregion
                    {

                        var result = mapper.Find<RolePower>(a => roleIds.Contains(a.RoleId), a => new { a.AppId });
                        //var sql = $@"SELECT a.AppId FROM RolePower AS a WHERE a.RoleId IN({String.Join(",", roleIds)}) AND a.IsDeleted=0";
                        //var result = mapper.Find<RolePower>(sql);
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
                using (var mapper = new EntityMapper())
                {

                    return mapper.Count<Role>(a => a.Name == name) > 0;
                    //var sql = $@"SELECT COUNT(*) FROM Role AS a WHERE a.Name=@name AND a.IsDeleted=0";
                    //var parameters = new List<EntityParameter>
                    //{
                    //    new EntityParameter("@name",name)
                    //};
                    //return mapper.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }

        public async Task<Boolean> CheckRoleIdentityAsync(String name)
        {
            Parameter.Validate(name);

            return await Task.Run(() =>
            {
                using (var mapper = new EntityMapper())
                {
                    return mapper.Count<Role>(a => a.RoleIdentity == name) > 0;
                    //var sql = $@"SELECT COUNT(*) FROM Role AS a WHERE a.RoleIdentity=@RoleIdentity AND a.IsDeleted=0";
                    //var parameters = new List<EntityParameter>
                    //{
                    //    new EntityParameter("@RoleIdentity",name)
                    //};
                    //return mapper.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }

        public async Task ModifyRoleAsync(Role role)
        {
            Parameter.Validate(role);

            await Task.Run(() =>
            {
                using (var mapper = new EntityMapper())
                {
                    #region 修改角色
                    {
                        role.ModifyRoleName(role.Name);
                        var result = mapper.Modify(role, r => r.Id == role.Id);
                        if (!result)
                        {
                            throw new BusinessException("修改角色失败");
                        }
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
                using (var mapper = new EntityMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        //var parameters = new List<EntityParameter>
                        //{
                        //    new EntityParameter("@roleId",roleId)
                        //};
                        #region 前置条件验证
                        {
                            var result = mapper.Count<UserRole>(a => a.RoleId == roleId);
                            //var sql = $@"SELECT COUNT(*) FROM UserRole AS a WHERE a.RoleId=@roleId AND a.IsDeleted=0";
                            //var result = mapper.FindSingleValue<Int32>(sql, parameters);
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
                            var result = mapper.Modify(role, r => r.Id == roleId);
                            if (!result)
                            {
                                throw new BusinessException("删除角色失败");
                            }
                        }
                        #endregion

                        #region 移除权限
                        {
                            var rolePower = new RolePower();
                            rolePower.Remove();
                            var result = mapper.Modify(rolePower, rp => rp.RoleId == roleId);
                            if (!result)
                            {
                                throw new BusinessException("移除权限失败");
                            }
                        }
                        #endregion

                        mapper.Commit();
                    }
                    catch (Exception)
                    {
                        mapper.Rollback();
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
                using (var mapper = new EntityMapper())
                {
                    #region 添加角色
                    {
                        mapper.Add(role);
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

                using (var mapper = new EntityMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        #region 移除之前的角色权限
                        {
                            var rolePower = new RolePower();
                            rolePower.Remove();
                            var result = mapper.Modify(rolePower, rp => rp.RoleId == roleId);
                            if (!result)
                            {
                                throw new BusinessException("移除之前的角色权限失败");
                            }
                        }
                        #endregion

                        #region 添加角色权限
                        {
                            var sqlBuilder = new StringBuilder();
                            foreach (var item in powerIds)
                            {
                                var rolePower = new RolePower(roleId, item);
                                mapper.Add(rolePower);
                            }
                        }
                        #endregion

                        mapper.Commit();
                    }
                    catch (Exception)
                    {
                        mapper.Rollback();
                        throw;
                    }
                }
            });
        }

        public async Task AddVisitorRecord(VisitorRecord visitorRecord)
        {
            Parameter.Validate(visitorRecord);

            await Task.Run(() =>
            {
                using (var mapper = new EntityMapper(Appsetting.Database))
                {
                    #region 添加角色
                    {
                        mapper.Add(visitorRecord);
                    }
                    #endregion
                }
            });
        }
    }
}
