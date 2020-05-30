using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Data.SQL;
using NewLibCore.Data.SQL.Filter;
using NewLibCore.Security;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class UserContext : IUserContext
    {
        public async Task<User> ValidateAsync(String userName, String password, String requestIp)
        {
            Parameter.Validate(userName);
            Parameter.Validate(password);

            return await Task.Run(() =>
             {
                 using (var mapper = EntityMapper.CreateMapper())
                 {
                     User user = null;
                     try
                     {
                         mapper.OpenTransaction();

                         #region 查询用户
                         {
                             user = mapper.Query<User>().InnerJoin<Config>((u, c) => u.Id == c.UserId)
                               .Where(u => u.Name == userName && !u.IsDisable)
                               .Select<User, Config>((u, c) => new
                               {
                                   u.Id,
                                   u.Name,
                                   u.LoginPassword,
                                   c.UserFace,
                                   u.IsAdmin,
                                   c.IsModifyUserFace
                               }).FirstOrDefault();

                             if (user == null)
                             {
                                 throw new BusinessException($"该用户不存在或被禁用{userName}");
                             }

                             if (!PasswordUtil.ComparePasswords(user.LoginPassword, password))
                             {
                                 throw new BusinessException("密码错误");
                             }
                         }
                         #endregion

                         #region 设置用户在线
                         {
                             user.Online();
                             var result = mapper.Update(user, acc => acc.Id == user.Id);
                             if (!result)
                             {
                                 throw new BusinessException("设置用户在线状态失败");
                             }
                         }
                         #endregion

                         #region 添加在线用户列表
                         {
                             var online = new Online(requestIp, user.Id);
                             mapper.Add(online);
                             if (online.Id == 0)
                             {
                                 throw new BusinessException("添加在线列表失败");
                             }
                         }
                         #endregion

                         mapper.Commit();
                         return user;
                     }
                     catch (Exception)
                     {
                         mapper.Rollback();
                         throw;
                     }
                 }
             });
        }

        public async Task<Config> GetConfigAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
             {
                 using (var mapper = EntityMapper.CreateMapper())
                 {
                     try
                     {


                         return mapper.Query<Config>()
                         .Where(w => w.UserId == userId)
                         .Select(a => new
                         {
                             a.Id,
                             a.Skin,
                             a.UserFace,
                             a.AppSize,
                             a.AppVerticalSpacing,
                             a.AppHorizontalSpacing,
                             a.DefaultDeskNumber,
                             a.DefaultDeskCount,
                             a.AppXy,
                             a.DockPosition,
                             a.WallpaperMode,
                             a.WallpaperId,
                             a.IsBing,
                             a.UserId,
                             a.IsModifyUserFace
                         }).FirstOrDefault();
                     }
                     catch (System.Exception)
                     {

                         throw;
                     }
                 }
             });
        }

        public async Task<Wallpaper> GetWallpaperAsync(Int32 wallPaperId)
        {
            Parameter.Validate(wallPaperId);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<Wallpaper>()
                        .Where(w => w.Id == wallPaperId)
                        .Select(a => new
                        {
                            a.Url,
                            a.Width,
                            a.Height,
                            a.Source
                        }).FirstOrDefault();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public List<User> GetUsers(String userName, String userType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);

            var where = FilterFactory.Create<User>();
            if (!String.IsNullOrEmpty(userName))
            {
                where.And(w => w.Name.Contains(userName));
            }

            if (!String.IsNullOrEmpty(userType))
            {
                var isAdmin = (EnumExtensions.ToEnum<UserType>(Int32.Parse(userType)) == UserType.Admin);
                where.And(w => w.IsAdmin == isAdmin);
            }

            using (var mapper = EntityMapper.CreateMapper())
            {
                try
                {


                    #region totalCount
                    {
                        totalCount = mapper.Query<User>()
                        .InnerJoin<Config>((a, b) => a.Id == b.UserId)
                        .Where(where).Count();
                    }
                    #endregion

                    #region sql
                    {
                        return mapper.Query<User>()
                        .InnerJoin<Config>((a, b) => a.Id == b.UserId)
                        .Where(where).Select<User, Config>((a, b) => new
                        {
                            a.Id,
                            a.IsAdmin,
                            a.Name,
                            a.IsDisable,
                            b.UserFace,
                            b.IsModifyUserFace
                        }).Page(pageIndex, pageSize)
                        .ThenByDesc<DateTime>(a => a.AddTime).ToList();
                    }
                    #endregion
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
        }

        public async Task<User> GetUserAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<User>()
                        .InnerJoin<Config>((a, b) => a.Id == b.UserId)
                        .Where(w => !w.IsDeleted)
                        .Select<User, Config>((a, b) => new
                        {
                            b.UserFace,
                            a.Id,
                            a.IsAdmin,
                            a.IsDisable,
                            a.IsOnline,
                            a.Name,
                            a.LockScreenPassword,
                            a.LoginPassword,
                            a.LastLoginTime,
                            a.AddTime,
                            b.IsModifyUserFace
                        }).FirstOrDefault();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<List<Role>> GetRolesAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<Role>()
                        .InnerJoin<UserRole>((a, b) => a.Id == b.RoleId)
                        .Where<UserRole>(a => a.UserId == userId)
                        .Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.RoleIdentity
                        })
                        .ThenByDesc<DateTime>(a => a.AddTime)
                        .ToList();
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
            });
        }

        public async Task<List<RolePower>> GetPowersAsync()
        {
            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<RolePower>()
                        .Select(a => new { a.RoleId, a.AppId })
                        .ThenByDesc<DateTime>(a => a.AddTime)
                        .ToList();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<Boolean> CheckUserNameExistAsync(String userName)
        {
            Parameter.Validate(userName);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<User>().Where(w => w.Name == userName).Count() > 0;

                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<String> GetOldPasswordAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run<String>(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<User>()
                        .Where(w => w.Id == userId && !w.IsDisable)
                        .Select(a => new { a.LoginPassword })
                        .FirstOrDefault().LoginPassword;
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task<Boolean> UnlockScreenAsync(Int32 userId, String unlockPassword)
        {
            Parameter.Validate(userId);
            Parameter.Validate(unlockPassword);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    #region 获取锁屏密码
                    {
                        var user = mapper.Query<User>()
                        .Where(w => w.Id == userId && !w.IsDisable)
                        .Select(a => new { a.LockScreenPassword })
                        .FirstOrDefault();

                        return PasswordUtil.ComparePasswords(user.LockScreenPassword, unlockPassword);
                    }
                    #endregion
                }
            });
        }

        public async Task<Boolean> CheckAppNameAsync(String name)
        {
            Parameter.Validate(name);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Query<App>().Where(w => w.Name == name).Count() > 0;
                }
            });
        }

        public async Task<Boolean> CheckAppUrlAsync(String url)
        {
            Parameter.Validate(url);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        return mapper.Query<App>().Where(w => w.AppUrl == url).Count() > 0;
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
            });
        }

        public async Task LogoutAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        mapper.OpenTransaction();
                        #region 设置用户下线
                        {
                            var user = new User();
                            user.Offline();
                            var result = mapper.Update(user, acc => acc.Id == userId && !acc.IsDeleted && !acc.IsDisable);
                            if (!result)
                            {
                                throw new BusinessException("设置用户下线状态失败");
                            }
                        }
                        #endregion

                        #region 将当前用户从在线列表中移除
                        {
                            var online = new Online();
                            online.Remove();
                            var result = mapper.Update(online, on => on.UserId == userId && !on.IsDeleted);
                            if (!result)
                            {
                                throw new BusinessException("将用户移出在线列表时失败");
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

        public async Task AddNewUserAsync(User user)
        {
            Parameter.Validate(user);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        mapper.OpenTransaction();

                        var userId = 0;
                        var configId = 0;

                        #region 新增用户
                        {
                            userId = mapper.Add(user).Id;

                            if (userId == 0)
                            {
                                throw new BusinessException("初始化用户时失败");
                            }
                        }
                        #endregion

                        #region 初始化配置
                        {
                            var config = new Config(userId);
                            configId = mapper.Add(config).Id;

                            if (configId == 0)
                            {
                                throw new BusinessException("初始化配置时失败");
                            }
                        }
                        #endregion

                        #region 更新用户的配置
                        {
                            user.ModifyConfigId(configId);
                            var result = mapper.Update(user, acc => acc.Id == userId);
                            if (!result)
                            {
                                throw new BusinessException("更新用户配置失败");
                            }
                        }
                        #endregion

                        #region 用户角色
                        {
                            var sqlBuilder = new StringBuilder();
                            foreach (var item in user.Roles)
                            {
                                mapper.Add(new UserRole(userId, item.RoleId));
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

        public async Task ModifyUserAsync(User user)
        {
            Parameter.Validate(user);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        if (!String.IsNullOrEmpty(user.LoginPassword))
                        {
                            #region 修改密码
                            {
                                var newPassword = PasswordUtil.CreateDbPassword(user.LoginPassword);
                                user.ModifyLoginPassword(newPassword);
                                var result = mapper.Update(user, acc => acc.Id == user.Id);
                                if (!result)
                                {
                                    throw new BusinessException("修改登陆密码失败");
                                }
                            }
                            #endregion
                        }

                        #region 修改用户角色
                        {
                            var userRole = new UserRole();
                            userRole.Remove();
                            var result = mapper.Update(userRole, acc => acc.UserId == user.Id);
                            if (!result)
                            {
                                throw new BusinessException("移除用户角色失败");
                            }
                            if (user.Roles == null || !user.Roles.Any())
                            {
                                user.DetachAdminRole();
                            }
                            else
                            {
                                user.AttachAdminRole();
                                foreach (var item in user.Roles)
                                {
                                    mapper.Add(new UserRole(user.Id, item.RoleId));
                                }
                            }
                            var result2 = mapper.Update(user, ac => ac.Id == user.Id);
                            if (!result2)
                            {
                                throw new BusinessException("修改用户角色失败");
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

        public async Task EnableAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        var user = new User().Enable();
                        var result = mapper.Update(user, acc => acc.Id == userId);
                        if (!result)
                        {
                            throw new BusinessException("用户启用失败");
                        }
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task DisableAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        #region 前置条件验证
                        {
                            var result = mapper.Query<Role>().InnerJoin<UserRole>((a, b) => a.Id == b.RoleId).Where<UserRole>((a, b) => a.IsAllowDisable && b.UserId == userId).Count();
                            if (result > 0)
                            {
                                throw new BusinessException("当前用户拥有管理员角色，因此不能禁用或删除");
                            }
                        }
                        #endregion
                        {
                            var user = new User().Disable();
                            var result = mapper.Update(user, acc => acc.Id == userId);
                            if (!result)
                            {
                                throw new BusinessException("用户启用失败");
                            }
                        }
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
            });
        }

        public async Task ModifyUserFaceAsync(Int32 userId, String newFace)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newFace);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        var config = new Config().ModifyUserFace(newFace);
                        var result = mapper.Update(config, conf => conf.UserId == userId);
                        if (!result)
                        {
                            throw new BusinessException("修改用户头像失败");
                        }
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
            });
        }

        public async Task ModifyPasswordAsync(Int32 userId, String newPassword, Boolean isTogetherSetLockPassword)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newPassword);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        var user = new User();
                        if (isTogetherSetLockPassword)
                        {
                            user.ModifyLockScreenPassword(newPassword);
                        }
                        user.ModifyLoginPassword(newPassword);
                        var result = mapper.Update(user, acc => acc.Id == userId && acc.IsDisable == false);
                        if (!result)
                        {
                            throw new BusinessException("修改登陆密码失败");
                        }
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task ModifyLockScreenPasswordAsync(Int32 userId, String newScreenPassword)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newScreenPassword);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    try
                    {
                        var user = new User().ModifyLockScreenPassword(newScreenPassword);
                        var result = mapper.Update(user, acc => acc.Id == userId && !acc.IsDisable);
                        if (!result)
                        {
                            throw new BusinessException("修改锁屏密码失败");
                        }
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            });
        }

        public async Task RemoveUserAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();

                    try
                    {
                        #region 前置条件验证
                        {
                            var isAdmin = mapper.Query<User>()
                            .Where(a => a.Id == userId && !a.IsDisable)
                            .Select(a => new { a.IsAdmin })
                            .FirstOrDefault().IsAdmin;
                            if (isAdmin)
                            {
                                throw new BusinessException("不能删除管理员");
                            }
                        }
                        #endregion

                        #region 移除账户
                        {
                            var user = new User();
                            user.Remove();
                            var result = mapper.Update(user, acc => acc.Id == userId && !acc.IsDisable);
                            if (!result)
                            {
                                throw new BusinessException("移除账户失败");
                            }
                        }
                        #endregion

                        #region 移除账户配置
                        {
                            var config = new Config();
                            config.Remove();
                            var result = mapper.Update(config, conf => conf.UserId == userId);
                            if (!result)
                            {
                                throw new BusinessException("移除账户配置失败");
                            }
                        }
                        #endregion

                        #region 移除用户角色
                        {
                            var userRole = new UserRole();
                            userRole.Remove();
                            var result = mapper.Update(userRole, accRole => accRole.UserId == userId);
                            if (!result)
                            {
                                throw new BusinessException("移除账户配置失败");
                            }
                        }
                        #endregion

                        #region 移除用户安装的app
                        {
                            var member = new Member();
                            member.Remove();
                            var result = mapper.Update(member, mem => mem.UserId == userId);
                            if (!result)
                            {
                                throw new BusinessException("移除账户配置失败");
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
    }
}
