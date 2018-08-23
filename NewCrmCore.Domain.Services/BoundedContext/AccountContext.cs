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
    public class AccountContext : IAccountContext
    {
        public async Task<Account> ValidateAsync(String accountName, String password, String requestIp)
        {
            Parameter.Validate(accountName);
            Parameter.Validate(password);

            return await Task.Run(() =>
             {
                 using (var dataStore = new DataStore(Appsetting.Database))
                 {
                     Account result = null;
                     try
                     {
                         dataStore.OpenTransaction();

                         #region 查询用户
                         {
                             var sql = @"SELECT a.Id,a.Name,a.LoginPassword,a1.AccountFace 
                                        FROM Account AS a
                                        INNER JOIN Config AS a1
                                        ON a1.AccountId=a.Id 
                                        WHERE a.Name=@name AND a.IsDeleted=0 AND a.IsDisable=0";
                             result = dataStore.Find<Account>(sql, new List<ParameterMapper> { new ParameterMapper("@name", accountName) }).FirstOrDefault();
                             if (result == null)
                             {
                                 throw new BusinessException($"该用户不存在或被禁用{accountName}");
                             }

                             if (!PasswordUtil.ComparePasswords(result.LoginPassword, password))
                             {
                                 throw new BusinessException("密码错误");
                             }
                         }
                         #endregion

                         #region 设置用户在线
                         {

                             result.Online();
                             var rowCount = dataStore.Modify(result, acc => acc.Id == result.Id);

                             if (rowCount == 0)
                             {
                                 throw new BusinessException("设置用户在线状态失败");
                             }
                         }
                         #endregion

                         #region 添加在线用户列表
                         {
                             var online = new Online(requestIp, result.Id);
                             var rowCount = dataStore.Add(online);

                             if (rowCount == 0)
                             {
                                 throw new BusinessException("添加在线列表失败");
                             }
                         }
                         #endregion

                         dataStore.Commit();
                         return result;
                     }
                     catch (Exception)
                     {
                         dataStore.Rollback();
                         throw;
                     }
                 }
             });
        }

        public async Task<Config> GetConfigAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            return await Task.Run(() =>
             {
                 using (var dataStore = new DataStore(Appsetting.Database))
                 {
                     var sql = $@"SELECT 
								a.Id,
								a.Skin,  
								a.AccountFace,
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
								a.AccountId,
                                a.IsModifyAccountFace,
                                a.WallpaperId
								FROM Config AS a WHERE a.AccountId=@accountId AND a.IsDeleted=0";
                     var parameters = new List<ParameterMapper> { new ParameterMapper("@accountId", accountId) };
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

        public List<Account> GetAccounts(String accountName, String accountType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);

            var where = new StringBuilder();
            where.Append("WHERE 1=1 AND a.IsDeleted=0 ");
            var parameters = new List<ParameterMapper>();
            if (!String.IsNullOrEmpty(accountName))
            {
                parameters.Add(new ParameterMapper("@name", accountName));
                where.Append(" AND a.Name=@name");
            }

            if (!String.IsNullOrEmpty(accountType))
            {
                var isAdmin = (EnumExtensions.ToEnum<AccountType>(Int32.Parse(accountType)) == AccountType.Admin) ? 1 : 0;
                parameters.Add(new ParameterMapper("@isAdmin", isAdmin));
                where.Append($@" AND a.IsAdmin=@isAdmin");
            }

            using (var dataStore = new DataStore(Appsetting.Database))
            {
                #region totalCount
                {
                    var sql = $@"SELECT COUNT(*) FROM Account AS a 
                                 INNER JOIN Config AS a1 ON a1.AccountId=a.Id AND a1.IsDeleted=0 {where} ";
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
								a1.AccountFace,
                                a1.IsModifyAccountFace
	                            FROM Account AS a 
	                            INNER JOIN Config AS a1
	                            ON a1.AccountId=a.Id AND a1.IsDeleted=0
	                            {where} LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    return dataStore.Find<Account>(sql, parameters);
                }
                #endregion
            }
        }

        public async Task<Account> GetAccountAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT 
                            a1.AccountFace,
                            a.AddTime,
                            a.Id,
                            a.IsAdmin,
                            a.IsDisable,
                            a.IsOnline,
                            a.LastLoginTime,
                            a.LastModifyTime,
                            a.Name,
                            a.LockScreenPassword,
                            a.LoginPassword
                            FROM Account AS a 
                            INNER JOIN  Config AS a1
                            ON a1.AccountId=a.Id
                            WHERE a.Id=@accountId AND a.IsDeleted=0 AND a.IsDisable=0";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@accountId", accountId) };
                    return dataStore.FindOne<Account>(sql, parameters);
                }
            });
        }

        public async Task<List<Role>> GetRolesAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT
								a1.Id,
								a1.Name,
								a1.RoleIdentity
								FROM AccountRole AS a
								INNER JOIN Role AS a1
								ON a1.Id=a.RoleId AND a1.IsDeleted=0 
								WHERE a.AccountId=@accountId AND a.IsDeleted=0 ";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@accountId", accountId) };
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

        public async Task<Boolean> CheckAccountNameExistAsync(String accountName)
        {
            Parameter.Validate(accountName);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM Account AS a WHERE a.Name=@name AND a.IsDeleted=0";
                    return dataStore.FindSingleValue<Int32>(sql, new List<ParameterMapper> { new ParameterMapper("@name", accountName) }) != 0 ? false : true;
                }
            });
        }

        public async Task<String> GetOldPasswordAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            return await Task.Run<String>(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT a.LoginPassword FROM Account AS a WHERE a.Id=@accountId AND a.IsDeleted=0 AND a.IsDisable=0";
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@accountId", accountId) };
                    return dataStore.FindSingleValue<String>(sql, parameters);
                }
            });
        }

        public async Task<Boolean> UnlockScreenAsync(Int32 accountId, String unlockPassword)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(unlockPassword);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region 获取锁屏密码
                    {
                        var sql = $@"SELECT a.LockScreenPassword FROM Account AS a WHERE a.Id=@accountId AND a.IsDeleted=0 AND a.IsDisable=0";
                        var parameters = new List<ParameterMapper>
                        {
                            new ParameterMapper("@accountId",accountId)
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

        public async Task LogoutAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    try
                    {
                        dataStore.OpenTransaction();
                        #region 设置用户下线
                        {
                            var account = new Account();
                            account.Offline();
                            var rowCount = dataStore.Modify(account, acc => acc.Id == accountId && !acc.IsDeleted && !acc.IsDisable);
                            if (rowCount == 0)
                            {
                                throw new BusinessException("设置用户下线状态失败");
                            }
                        }
                        #endregion

                        #region 将当前用户从在线列表中移除
                        {
                            var online = new Online();
                            online.Remove();
                            var rowCount = dataStore.Modify(online, on => on.AccountId == accountId && !on.IsDeleted);
                            if (rowCount == 0)
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

        public async Task AddNewAccountAsync(Account account)
        {
            Parameter.Validate(account);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    try
                    {
                        dataStore.OpenTransaction();

                        var accountId = 0;
                        var configId = 0;

                        #region 新增用户
                        {
                            accountId = dataStore.Add(account);

                            if (accountId == 0)
                            {
                                throw new BusinessException("初始化用户时失败");
                            }
                        }
                        #endregion

                        #region 初始化配置
                        {
                            var config = new Config(accountId);
                            configId = dataStore.Add(config);

                            if (configId == 0)
                            {
                                throw new BusinessException("初始化配置时失败");
                            }
                        }
                        #endregion

                        #region 更新用户的配置
                        {
                            account.ModifyConfigId(configId);
                            var accountRowCount = dataStore.Modify(account, acc => acc.Id == accountId);
                            if (accountRowCount == 0)
                            {
                                throw new BusinessException("更新用户配置失败");
                            }
                        }
                        #endregion

                        #region 用户角色
                        {
                            var sqlBuilder = new StringBuilder();
                            foreach (var item in account.Roles)
                            {
                                dataStore.Add(new AccountRole(accountId, item.RoleId));
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

        public async Task ModifyAccountAsync(Account account)
        {
            Parameter.Validate(account);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        if (!String.IsNullOrEmpty(account.LoginPassword))
                        {
                            #region 修改密码
                            {
                                var newPassword = PasswordUtil.CreateDbPassword(account.LoginPassword);
                                account.ModifyLoginPassword(newPassword);

                                var rowCount = dataStore.Modify(account, acc => acc.Id == account.Id);
                                if (rowCount == 0)
                                {
                                    throw new BusinessException("修改登陆密码失败");
                                }
                            }
                            #endregion
                        }

                        #region 修改账户角色
                        {
                            var accountRole = new AccountRole();
                            accountRole.Remove();
                            dataStore.Modify(accountRole, acc => acc.AccountId == account.Id);

                            if (account.Roles == null || !account.Roles.Any())
                            {
                                account.DetachAdminRole();
                            }
                            else
                            {
                                account.AttachAdminRole();
                                foreach (var item in account.Roles)
                                {
                                    dataStore.Add(new AccountRole(account.Id, item.RoleId));
                                }
                            }
                            dataStore.Modify(account, ac => ac.Id == account.Id);

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

        public async Task EnableAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var account = new Account().Enable();
                    dataStore.Modify(account, acc => acc.Id == accountId);
                }
            });
        }

        public async Task DisableAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var parameters = new List<ParameterMapper> { new ParameterMapper("@accountId", accountId) };
                    #region 前置条件验证
                    {
                        var sql = $@"SELECT COUNT(*) FROM Role AS a
									INNER JOIN AccountRole AS a1
									ON a1.AccountId=@accountId AND a1.RoleId=a.Id AND a1.IsDeleted=0
									WHERE a.IsDeleted=0 AND a.IsAllowDisable=0";
                        var result = dataStore.FindSingleValue<Int32>(sql, parameters);
                        if (result > 0)
                        {
                            throw new BusinessException("当前用户拥有管理员角色，因此不能禁用或删除");
                        }
                    }
                    #endregion
                    {
                        var account = new Account().Disable();
                        dataStore.Modify(account, acc => acc.Id == accountId);
                    }
                }
            });
        }

        public async Task ModifyAccountFaceAsync(Int32 accountId, String newFace)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(newFace);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var config = new Config().ModifyAccountFace(newFace);
                    dataStore.Modify(config, conf => conf.AccountId == accountId);
                }
            });
        }

        public async Task ModifyPasswordAsync(Int32 accountId, String newPassword, Boolean isTogetherSetLockPassword)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(newPassword);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var account = new Account();
                    if (isTogetherSetLockPassword)
                    {
                        account.ModifyLockScreenPassword(newPassword);
                    }
                    account.ModifyLoginPassword(newPassword);
                    dataStore.Modify(account, acc => acc.Id == accountId && acc.IsDisable == false);
                }
            });
        }

        public async Task ModifyLockScreenPasswordAsync(Int32 accountId, String newScreenPassword)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(newScreenPassword);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var account = new Account().ModifyLockScreenPassword(newScreenPassword);
                    dataStore.Modify(account, acc => acc.Id == accountId && !acc.IsDisable);
                }
            });
        }

        public async Task RemoveAccountAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();

                    try
                    {
                        #region 前置条件验证
                        {
                            var parameters = new List<ParameterMapper>
                            {
                                new ParameterMapper("@accountId",accountId)
                            };

                            var sql = $@"SELECT a.IsAdmin FROM Account AS a WHERE a.Id=@accountId AND a.IsDeleted=0 AND a.IsDisable=0";
                            var isAdmin = dataStore.FindSingleValue<Boolean>(sql, parameters);
                            if (isAdmin)
                            {
                                throw new BusinessException("不能删除管理员");
                            }
                        }
                        #endregion

                        #region 移除账户
                        {
                            var account = new Account();
                            account.Remove();
                            dataStore.Modify(account, acc => acc.Id == accountId && !acc.IsDisable);
                        }
                        #endregion

                        #region 移除账户配置
                        {
                            var config = new Config();
                            config.Remove();
                            dataStore.Modify(config, conf => conf.AccountId == accountId);
                        }
                        #endregion

                        #region 移除用户角色
                        {
                            var accountRole = new AccountRole();
                            accountRole.Remove();
                            dataStore.Modify(accountRole, accRole => accRole.AccountId == accountId);
                        }
                        #endregion

                        #region 移除用户安装的app
                        {
                            var member = new Member();
                            member.Remove();
                            dataStore.Modify(member, mem => mem.AccountId == accountId);
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
