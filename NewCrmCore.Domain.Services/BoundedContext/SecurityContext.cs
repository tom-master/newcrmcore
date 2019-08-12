using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools; 
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Data.SQL.MergeExpression;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class SecurityContext : ISecurityContext
    {
        public async Task<List<RolePower>> GetPowersAsync()
        {
            return await Task.Run(() =>
            {
                using(var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Select<RolePower>(a => new
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
                using(var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Select<Role>(a => new { a.Id, a.Name, a.RoleIdentity }).Where(a => a.Id == roleId).FirstOrDefault();
                    //var sql = $@"SELECT a.Id, a.Name, a.RoleIdentity, a.Remark FROM Role AS a WHERE a.Id=@Id AND a.IsDeleted=0";
                    //var parameters = new List<EntityParameter> { new EntityParameter("@Id", roleId) };
                    //return mapper.FindOne<Role>(sql, parameters);
                }
            });
        }

        public List<Role> GetRoles(String roleName, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            using(var mapper = EntityMapper.CreateMapper())
            {
                // var where = new StringBuilder();
                // var parameters = new List<EntityParameter>();
                var where = MergeFactory.Create<Role>();
                if (!String.IsNullOrEmpty(roleName))
                {
                    where.And(w => w.Name.Contains(roleName));
                    // parameters.Add(new EntityParameter("@roleName", $@"%{roleName}%"));
                    // where.Append($@" AND a.Name LIKE @roleName");
                }

                #region totalCount
                {
                    totalCount = mapper.Select<Role>().Where(where).Count();
                    // var sql = $@"SELECT COUNT(*) FROM newcrm_role AS a WHERE 1=1 {where} AND a.IsDeleted=0";
                    // totalCount = mapper.ExecuteToSingle<Int32>(sql, parameters);
                }
                #endregion

                #region sql
                {
                    return mapper.Select<Role>(a => new
                    {
                        a.Name,
                        a.RoleIdentity,
                        a.Remark,
                        a.Id
                    }).Where(where).ToList();
                    // var sql = $@"SELECT
                    // 			a.Name,
                    // 			a.RoleIdentity,
                    // 			a.Remark,
                    // 			a.Id
                    // 			FROM newcrm_role AS a WHERE 1=1 {where} AND a.IsDeleted=0 LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    // return mapper.ExecuteToList<Role>(sql, parameters);
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
                using(var mapper = EntityMapper.CreateMapper())
                {
                    #region 检查app是否为系统app
                    {
                        var result = mapper.Select<App>().Where(a => a.Id == accessAppId && a.IsSystem).Count();
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
                        var result = mapper.Select<RolePower>(a => new { a.AppId }).Where(a => roleIds.Contains(a.RoleId)).ToList();
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
                using(var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Select<Role>().Where(a => a.Name == name).Exist();
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
                using(var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Select<Role>().Where(a => a.RoleIdentity == name).Exist();
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
                using(var mapper = EntityMapper.CreateMapper())
                {
                    #region 修改角色
                    {
                        role.ModifyRoleName(role.Name);
                        var result = mapper.Update(role, r => r.Id == role.Id);
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
                using(var mapper = EntityMapper.CreateMapper())
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
                            var result = mapper.Select<UserRole>().Where(a => a.RoleId == roleId).Count();
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
                            var result = mapper.Update(role, r => r.Id == roleId);
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
                            var result = mapper.Update(rolePower, rp => rp.RoleId == roleId);
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
                using(var mapper = EntityMapper.CreateMapper())
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

                using(var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        #region 移除之前的角色权限
                        {
                            var rolePower = new RolePower();
                            rolePower.Remove();
                            var result = mapper.Update(rolePower, rp => rp.RoleId == roleId);
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
                using(var mapper = EntityMapper.CreateMapper())
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
