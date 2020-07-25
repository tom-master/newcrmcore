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
using NewLibCore.Data.SQL;
using NewLibCore.Data.SQL.Filter;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class SecurityContext : ISecurityContext
    {
        public async Task<List<RolePower>> GetPowersAsync()
        {
            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        return mapper.Query<RolePower>().Select(a => new
                        {
                            a.RoleId,
                            a.AppId
                        }).ToList();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<Role> GetRoleAsync(Int32 roleId)
        {
            Parameter.Validate(roleId);

            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        return mapper.Query<Role>()
                        .Where(a => a.Id == roleId)
                        .Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.RoleIdentity
                        }).FirstOrDefault();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public List<Role> GetRoles(String roleName, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            using var mapper = EntityMapper.CreateMapper();
            {
                try
                {


                    var where = FilterFactory.Create<Role>();
                    if (!String.IsNullOrEmpty(roleName))
                    {
                        where.And(w => w.Name.Contains(roleName));
                    }

                    #region totalCount
                    {
                        totalCount = mapper.Query<Role>().Where(where).Count();
                    }
                    #endregion

                    #region sql
                    {
                        return mapper.Query<Role>().Where(where)
                        .Select(a => new
                        {
                            a.Name,
                            a.RoleIdentity,
                            a.Remark,
                            a.Id
                        }).ToList();
                    }
                    #endregion
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }

        public async Task<Boolean> CheckPermissionsAsync(Int32 accessAppId, params Int32[] roleIds)
        {
            Parameter.Validate(accessAppId);
            Parameter.Validate(roleIds);

            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        #region 检查app是否为系统app
                        {
                            var result = mapper.Query<App>().Where(a => a.Id == accessAppId && a.IsSystem).Select().Count();
                            if (result <= 0)
                            {
                                return true;
                            }
                        }
                        #endregion
                        {
                            var result = mapper.Query<RolePower>().Where(a => roleIds.Contains(a.RoleId)).Select(a => new { a.AppId }).ToList();
                            return result.Any(a => a.AppId == accessAppId);
                        }
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<Boolean> CheckRoleNameAsync(String name)
        {
            Parameter.Validate(name);

            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        return mapper.Query<Role>().Where(a => a.Name == name).Count() > 0;
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<Boolean> CheckRoleIdentityAsync(String name)
        {
            Parameter.Validate(name);

            return await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        return mapper.Query<Role>().Where(a => a.RoleIdentity == name).Count() > 0;
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task ModifyRoleAsync(Role role)
        {
            Parameter.Validate(role);

            await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
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
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
            });
        }

        public async Task RemoveRoleAsync(Int32 roleId)
        {
            Parameter.Validate(roleId);

            await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                {
                    mapper.OpenTransaction();
                    try
                    {
                        #region 前置条件验证
                        {
                            var result = mapper.Query<UserRole>().Where(a => a.RoleId == roleId).Count();
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
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        #region 添加角色
                        {
                            mapper.Add(role);
                        }
                        #endregion
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }

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

                using var mapper = EntityMapper.CreateMapper();
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
                using var mapper = EntityMapper.CreateMapper();
                {
                    try
                    {
                        #region 添加角色
                        {
                            mapper.Add(visitorRecord);
                        }
                        #endregion
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }
    }
}
