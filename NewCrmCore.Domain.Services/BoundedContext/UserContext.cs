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
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Data.Mapper.InternalDataStore;
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
                 using (var dataStore = new DataStore(Appsetting.Database))
                 {
                     User user = null;
                     try
                     {
                         dataStore.OpenTransaction();

                         #region 查询用户
                         {
                             var sql = @"SELECT a.Id,a.Name,a.LoginPassword,a1.UserFace,a.IsAdmin
                                        FROM User AS a
                                        INNER JOIN Config AS a1
                                        ON a1.UserId=a.Id 
                                        WHERE a.Name=@name AND a.IsDeleted=0 AND a.IsDisable=0";
                             user = dataStore.Find<User>(sql, new List<ParameterMapper> { new ParameterMapper("@name", userName) }).FirstOrDefault();
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
                             var result = dataStore.Modify(user, acc => acc.Id == user.Id);
                             if (!result)
                             {
                                 throw new BusinessException("设置用户在线状态失败");
                             }
                         }
                         #endregion

                         #region 添加在线用户列表
                         {
                             var online = new Online(requestIp, user.Id);
                             var rowCount = dataStore.Add(online);

                             if (rowCount == 0)
                             {
                                 throw new BusinessException("添加在线列表失败");
                             }
                         }
                         #endregion

                         dataStore.Commit();
                         return user;
                     }
                     catch (Exception)
                     {
                         dataStore.Rollback();
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
                 using (var dataStore = new DataStore(Appsetting.Database))
                 {
                     var sql = $@"SELECT 
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
                                a.IsModifyUserFace,
                                a.WallpaperId
								FROM Config AS a WHERE a.UserId=@userId AND a.IsDeleted=0";
                     var parameters = new List<ParameterMapper> { new ParameterMapper("@userId", userId) };
                     var result = dataStore.Find<Config>(sql, parameters).FirstOrDefault();
                     return result;
                 }
             });
        }

        public async Task<Wallpaper> GetWallpaperAsync(Int32 wallPaperId)
        {
            Parameter.Validate(wallPaperId);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.Url,a.Width,a.Height,a.Source FROM Wallpaper AS a WHERE a.Id=@wallpaperId AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@wallpaperId", wallPaperId) };
                    return dataStore.Find<Wallpaper>(sql, parameters).FirstOrDefault();
                }
            });
        }

        public List<User> GetUsers(String userName, String userType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);

            var where = new StringBuilder();
            where.Append("WHERE 1=1 AND a.IsDeleted=0 ");
            var parameters = new List<ParameterMapper>();
            if (!String.IsNullOrEmpty(userName))
            {
                parameters.Add(new ParameterMapper("@name", userName));
                where.Append(" AND a.Name=@name");
            }

            if (!String.IsNullOrEmpty(userType))
            {
                var isAdmin = (EnumExtensions.ToEnum<UserType>(Int32.Parse(userType)) == UserType.Admin) ? 1 : 0;
                parameters.Add(new ParameterMapper("@isAdmin", isAdmin));
                where.Append($@" AND a.IsAdmin=@isAdmin");
            }

            using (var dataStore = new DataStore(Appsetting.Database))
            {
                #region totalCount
                {
                    var sql = $@"SELECT COUNT(*) FROM User AS a INNER JOIN Config AS a1 ON a1.UserId=a.Id AND a1.IsDeleted=0 {where} ";
                    totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
                }
                #endregion

                #region sql
                {
                    var sql = $@"SELECT 
								a.Id,
								a.IsAdmin,
								a.Name,
								a.IsDisable,
								a1.UserFace,
                                a1.IsModifyUserFace
	                            FROM User AS a 
	                            INNER JOIN Config AS a1
	                            ON a1.UserId=a.Id AND a1.IsDeleted=0
	                            {where} LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    return dataStore.Find<User>(sql, parameters);
                }
                #endregion
            }
        }

        public async Task<User> GetUserAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT 
                            a1.UserFace,
                            a.AddTime,
                            a.Id,
                            a.IsAdmin,
                            a.IsDisable,
                            a.IsOnline,
                            a.LastLoginTime,
                            a.LastModifyTime,
                            a.Name,
                            a.LockScreenPassword,
                            a.LoginPassword,
                            a1.IsModifyUserFace
                            FROM User AS a 
                            INNER JOIN  Config AS a1
                            ON a1.UserId=a.Id
                            WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@userId", userId) };
                    return dataStore.FindOne<User>(sql, parameters);
                }
            });
        }

        public async Task<List<Role>> GetRolesAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT
								a1.Id,
								a1.Name,
								a1.RoleIdentity
								FROM UserRole AS a
								INNER JOIN Role AS a1
								ON a1.Id=a.RoleId AND a1.IsDeleted=0 
								WHERE a.UserId=@userId AND a.IsDeleted=0 ";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@userId", userId) };
                    return dataStore.Find<Role>(sql, parameters);
                }
            });
        }

        public async Task<List<RolePower>> GetPowersAsync()
        {
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.RoleId,a.AppId FROM RolePower AS a WHERE a.IsDeleted=0";
                    return dataStore.Find<RolePower>(sql);
                }
            });
        }

        public async Task<Boolean> CheckUserNameExistAsync(String userName)
        {
            Parameter.Validate(userName);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM User AS a WHERE a.Name=@name AND a.IsDeleted=0";
                    return dataStore.FindSingleValue<Int32>(sql, new List<ParameterMapper> { new ParameterMapper("@name", userName) }) != 0 ? false : true;
                }
            });
        }

        public async Task<String> GetOldPasswordAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run<String>(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.LoginPassword FROM User AS a WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@userId", userId) };
                    return dataStore.FindSingleValue<String>(sql, parameters);
                }
            });
        }

        public async Task<Boolean> UnlockScreenAsync(Int32 userId, String unlockPassword)
        {
            Parameter.Validate(userId);
            Parameter.Validate(unlockPassword);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 获取锁屏密码
                    {
                        var sql = $@"SELECT a.LockScreenPassword FROM User AS a WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@userId",userId)
                        };
                        var password = dataStore.FindSingleValue<String>(sql, parameters);
                        return PasswordUtil.ComparePasswords(password, unlockPassword);
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
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.Name=@name AND a.IsDeleted=0 ";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@name",name)
                    };
                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
                }

            });
        }

        public async Task<Boolean> CheckAppUrlAsync(String url)
        {
            Parameter.Validate(url);

            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.AppUrl = @url AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@url",url)
                    };

                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }

        public async Task LogoutAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    try
                    {
                        dataStore.OpenTransaction();
                        #region 设置用户下线
                        {
                            var user = new User();
                            user.Offline();
                            var result = dataStore.Modify(user, acc => acc.Id == userId && !acc.IsDeleted && !acc.IsDisable);
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
                            var result = dataStore.Modify(online, on => on.UserId == userId && !on.IsDeleted);
                            if (!result)
                            {
                                throw new BusinessException("将用户移出在线列表时失败");
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

        public async Task AddNewUserAsync(User user)
        {
            Parameter.Validate(user);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    try
                    {
                        dataStore.OpenTransaction();

                        var userId = 0;
                        var configId = 0;

                        #region 新增用户
                        {
                            userId = dataStore.Add(user);

                            if (userId == 0)
                            {
                                throw new BusinessException("初始化用户时失败");
                            }
                        }
                        #endregion

                        #region 初始化配置
                        {
                            var config = new Config(userId);
                            configId = dataStore.Add(config);

                            if (configId == 0)
                            {
                                throw new BusinessException("初始化配置时失败");
                            }
                        }
                        #endregion

                        #region 更新用户的配置
                        {
                            user.ModifyConfigId(configId);
                            var result = dataStore.Modify(user, acc => acc.Id == userId);
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
                                dataStore.Add(new UserRole(userId, item.RoleId));
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

        public async Task ModifyUserAsync(User user)
        {
            Parameter.Validate(user);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        if (!String.IsNullOrEmpty(user.LoginPassword))
                        {
                            #region 修改密码
                            {
                                var newPassword = PasswordUtil.CreateDbPassword(user.LoginPassword);
                                user.ModifyLoginPassword(newPassword);
                                var result = dataStore.Modify(user, acc => acc.Id == user.Id);
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
                            var result = dataStore.Modify(userRole, acc => acc.UserId == user.Id);
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
                                    dataStore.Add(new UserRole(user.Id, item.RoleId));
                                }
                            }
                            var result2 = dataStore.Modify(user, ac => ac.Id == user.Id);
                            if (!result2)
                            {
                                throw new BusinessException("修改用户角色失败");
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

        public async Task EnableAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var user = new User().Enable();
                    var result = dataStore.Modify(user, acc => acc.Id == userId);
                    if (!result)
                    {
                        throw new BusinessException("用户启用失败");
                    }
                }
            });
        }

        public async Task DisableAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@userId", userId) };
                    #region 前置条件验证
                    {
                        var sql = $@"SELECT COUNT(*) FROM Role AS a INNER JOIN UserRole AS a1 ON a1.UserId=@userId AND a1.RoleId=a.Id AND a1.IsDeleted=0
									 WHERE a.IsDeleted=0 AND a.IsAllowDisable=0";
                        var result = dataStore.FindSingleValue<Int32>(sql, parameters);
                        if (result > 0)
                        {
                            throw new BusinessException("当前用户拥有管理员角色，因此不能禁用或删除");
                        }
                    }
                    #endregion
                    {
                        var user = new User().Disable();
                        var result = dataStore.Modify(user, acc => acc.Id == userId);
                        if (!result)
                        {
                            throw new BusinessException("用户启用失败");
                        }
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
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config().ModifyUserFace(newFace);
                    var result = dataStore.Modify(config, conf => conf.UserId == userId);
                    if (!result)
                    {
                        throw new BusinessException("修改用户头像失败");
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
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var user = new User();
                    if (isTogetherSetLockPassword)
                    {
                        user.ModifyLockScreenPassword(newPassword);
                    }
                    user.ModifyLoginPassword(newPassword);
                    var result = dataStore.Modify(user, acc => acc.Id == userId && acc.IsDisable == false);
                    if (!result)
                    {
                        throw new BusinessException("修改登陆密码失败");
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
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var user = new User().ModifyLockScreenPassword(newScreenPassword);
                    var result = dataStore.Modify(user, acc => acc.Id == userId && !acc.IsDisable);
                    if (!result)
                    {
                        throw new BusinessException("修改锁屏密码失败");
                    }
                }
            });
        }

        public async Task RemoveUserAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();

                    try
                    {
                        #region 前置条件验证
                        {
                            var sql = $@"SELECT a.IsAdmin FROM User AS a WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
                            var parameters = new List<ParameterMapper>
                            {
                                new ParameterMapper("@userId",userId)
                            };
                            var isAdmin = dataStore.FindSingleValue<Boolean>(sql, parameters);
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
                            var result = dataStore.Modify(user, acc => acc.Id == userId && !acc.IsDisable);
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
                            var result = dataStore.Modify(config, conf => conf.UserId == userId);
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
                            var result = dataStore.Modify(userRole, accRole => accRole.UserId == userId);
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
                            var result = dataStore.Modify(member, mem => mem.UserId == userId);
                            if (!result)
                            {
                                throw new BusinessException("移除账户配置失败");
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
