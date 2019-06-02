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
using NewLibCore.Data.SQL.Mapper;
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
				 using (var mapper = new EntityMapper())
				 {
					 User user = null;
					 try
					 {
						 mapper.OpenTransaction();

						 #region 查询用户
						 {
							 user = mapper.Select<User,Config>((a,b) => new
							 {
								 a.Id,
								 a.Name,
								 a.LoginPassword,
								 b.UserFace,
								 a.IsAdmin,
								 b.IsModifyUserFace
							 }).InnerJoin<Config>((a, b) => a.Id == b.UserId).Where(a => a.Name == userName && !a.IsDisable).FirstOrDefault();
							 //user = mapper.InnerJoin<User, Config>((u, c) => u.Id == c.UserId).Find<User>(a => a.Name == userName && !a.IsDisable, a => new
							 //{
							 // a.Id,
							 // a.Name,
							 // a.LoginPassword,
							 // a.UserFace,
							 // a.IsAdmin,
							 // a.IsModifyUserFace
							 //}).FirstOrDefault();

							 //  var sql = @"SELECT a.Id,a.Name,a.LoginPassword,a1.UserFace,a.IsAdmin,a1.IsModifyUserFace
							 //             FROM User AS a
							 //             INNER JOIN Config AS a1
							 //             ON a1.UserId=a.Id 
							 //             WHERE a.Name=@name AND a.IsDeleted=0 AND a.IsDisable=0";
							 //  user = mapper.Find<User>(sql, new List<EntityParameter> { new EntityParameter("@name", userName) }).FirstOrDefault();
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
							 var result = mapper.Modify(user, acc => acc.Id == user.Id);
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
				 using (var mapper = new EntityMapper())
				 {
					 return mapper.Select<Config>(a => new
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
					 }).Where(w => w.UserId == userId).FirstOrDefault();
					 //  var sql = $@"SELECT 
					 // 			a.Id,
					 // 			a.Skin,  
					 // 			a.UserFace,
					 // 			a.AppSize,  
					 // 			a.AppVerticalSpacing,
					 // 			a.AppHorizontalSpacing,
					 // 			a.DefaultDeskNumber,
					 // 			a.DefaultDeskCount,
					 // 			a.AppXy,
					 // 			a.DockPosition,
					 // 			a.WallpaperMode,
					 // 			a.WallpaperId,
					 // 			a.IsBing,
					 // 			a.UserId,
					 //             a.IsModifyUserFace,
					 //             a.WallpaperId
					 // 			FROM Config AS a WHERE a.UserId=@userId AND a.IsDeleted=0";
					 //  var parameters = new List<EntityParameter> { new EntityParameter("@userId", userId) };
					 //  var result = mapper.Find<Config>(sql, parameters).FirstOrDefault();
					 //  return result;
				 }
			 });
		}

		public async Task<Wallpaper> GetWallpaperAsync(Int32 wallPaperId)
		{
			Parameter.Validate(wallPaperId);

			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<Wallpaper>(a => new
					{
						a.Url,
						a.Width,
						a.Height,
						a.Source
					}).Where(w => w.Id == wallPaperId).FirstOrDefault();

					// var sql = $@"SELECT a.Url,a.Width,a.Height,a.Source FROM Wallpaper AS a WHERE a.Id=@wallpaperId AND a.IsDeleted=0";
					// var parameters = new List<EntityParameter> { new EntityParameter("@wallpaperId", wallPaperId) };
					// return mapper.Find<Wallpaper>(sql, parameters).FirstOrDefault();
				}
			});
		}

		public List<User> GetUsers(String userName, String userType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
		{
			Parameter.Validate(pageIndex);
			Parameter.Validate(pageSize);

			var where = new StringBuilder();
			where.Append("WHERE 1=1 AND a.IsDeleted=0 ");
			var parameters = new List<EntityParameter>();
			if (!String.IsNullOrEmpty(userName))
			{
				parameters.Add(new EntityParameter("@name", userName));
				where.Append(" AND a.Name=@name");
			}

			if (!String.IsNullOrEmpty(userType))
			{
				var isAdmin = (EnumExtensions.ToEnum<UserType>(Int32.Parse(userType)) == UserType.Admin) ? 1 : 0;
				parameters.Add(new EntityParameter("@isAdmin", isAdmin));
				where.Append($@" AND a.IsAdmin=@isAdmin");
			}

			using (var mapper = new EntityMapper())
			{
				#region totalCount
				{
					var sql = $@"SELECT COUNT(*) FROM newcrm_user AS a INNER JOIN newcrm_user_config AS a1 ON a1.UserId=a.Id AND a1.IsDeleted=0 {where} ";
					totalCount = mapper.ExecuteToSingle<Int32>(sql, parameters);
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
	                            FROM newcrm_user AS a 
	                            INNER JOIN newcrm_user_config AS a1
	                            ON a1.UserId=a.Id AND a1.IsDeleted=0
	                            {where} LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
					return mapper.ExecuteToList<User>(sql, parameters);
				}
				#endregion
			}
		}

		public async Task<User> GetUserAsync(Int32 userId)
		{
			Parameter.Validate(userId);

			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<User,Config>((a,b) => new
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
					}).InnerJoin<Config>((a, b) => a.Id == b.UserId).Where(w => !w.IsDeleted).FirstOrDefault();


					//return mapper.InnerJoin<User, Config>((a, b) => a.Id == b.UserId).Find<User>(a => !a.IsDisable, a => new
					//{
					//	a.UserFace,
					//	a.Id,
					//	a.IsAdmin,
					//	a.IsDisable,
					//	a.IsOnline,
					//	a.Name,
					//	a.LockScreenPassword,
					//	a.LoginPassword,
					//	a.LastLoginTime,
					//	a.AddTime,
					//	a.IsModifyUserFace
					//}).FirstOrDefault();

					//var sql = $@"SELECT 
					//        a1.UserFace,
					//        a.Id,
					//        a.IsAdmin,
					//        a.IsDisable,
					//        a.IsOnline,
					//        a.Name,
					//        a.LockScreenPassword,
					//        a.LoginPassword,
					//        a.LastLoginTime,
					//        a.AddTime,
					//        a1.IsModifyUserFace
					//        FROM User AS a 
					//        INNER JOIN  Config AS a1
					//        ON a1.UserId=a.Id
					//        WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
					//var parameters = new List<EntityParameter> { new EntityParameter("@userId", userId) };
					//return mapper.ComplexSqlExecute<User>(sql, parameters);
				}
			});
		}

		public async Task<List<Role>> GetRolesAsync(Int32 userId)
		{
			Parameter.Validate(userId);

			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<Role>(a => new
					{
						a.Id,
						a.Name,
						a.RoleIdentity
					}).InnerJoin<UserRole>((a, b) => a.Id == b.RoleId).Where<UserRole>(a => a.UserId == userId).ToList();

					//mapper.InnerJoin<Role, UserRole>((a, b) => a.Id == b.RoleId).Find<Role>(a => a);

					//var sql = $@"SELECT
					//			a1.Id,
					//			a1.Name,
					//			a1.RoleIdentity
					//			FROM UserRole AS a
					//			INNER JOIN Role AS a1
					//			ON a1.Id=a.RoleId AND a1.IsDeleted=0 
					//			WHERE a.UserId=@userId AND a.IsDeleted=0 ";
					//var parameters = new List<EntityParameter> { new EntityParameter("@userId", userId) };
					//return mapper.Find<Role>(sql, parameters);
				}
			});
		}

		public async Task<List<RolePower>> GetPowersAsync()
		{
			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<RolePower>(a => new { a.RoleId, a.AppId }).ToList();
					//var sql = $@"SELECT a.RoleId,a.AppId FROM RolePower AS a WHERE a.IsDeleted=0";
					//return mapper.Find<RolePower>(sql);
				}
			});
		}

		public async Task<Boolean> CheckUserNameExistAsync(String userName)
		{
			Parameter.Validate(userName);

			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return !(mapper.Select<User>().Where(w => w.Name == userName).Exist());

					//var sql = $@"SELECT COUNT(*) FROM User AS a WHERE a.Name=@name AND a.IsDeleted=0";
					//return mapper.FindSingleValue<Int32>(sql, new List<EntityParameter> { new EntityParameter("@name", userName) }) != 0 ? false : true;
				}
			});
		}

		public async Task<String> GetOldPasswordAsync(Int32 userId)
		{
			Parameter.Validate(userId);

			return await Task.Run<String>(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<User>(a => new { a.LoginPassword }).Where(w => w.Id == userId && !w.IsDisable).FirstOrDefault().LoginPassword;
					//var sql = $@"SELECT a.LoginPassword FROM User AS a WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
					//var parameters = new List<EntityParameter> { new EntityParameter("@userId", userId) };
					//return mapper.FindSingleValue<String>(sql, parameters);
				}
			});
		}

		public async Task<Boolean> UnlockScreenAsync(Int32 userId, String unlockPassword)
		{
			Parameter.Validate(userId);
			Parameter.Validate(unlockPassword);

			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					#region 获取锁屏密码
					{
						//var sql = $@"SELECT a.LockScreenPassword FROM User AS a WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
						//var parameters = new List<EntityParameter>
						//{
						//	new EntityParameter("@userId",userId)
						//};
						//var password = mapper.FindSingleValue<String>(sql, parameters);
						var user = mapper.Select<User>(a => new { a.LockScreenPassword }).Where(w => w.Id == userId && !w.IsDisable).FirstOrDefault();
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
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<App>().Where(w => w.Name == name).Exist();
					//var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.Name=@name AND a.IsDeleted=0 ";
					//var parameters = new List<EntityParameter>
					//{
					//	new EntityParameter("@name",name)
					//};
					//return mapper.FindSingleValue<Int32>(sql, parameters) > 0;
				}

			});
		}

		public async Task<Boolean> CheckAppUrlAsync(String url)
		{
			Parameter.Validate(url);

			return await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					return mapper.Select<App>().Where(w => w.AppUrl == url).Exist();
					//var sql = $@"SELECT COUNT(*) FROM App AS a WHERE a.AppUrl = @url AND a.IsDeleted=0";
					//var parameters = new List<EntityParameter>
					//{
					//	new EntityParameter("@url",url)
					//};

					//return mapper.FindSingleValue<Int32>(sql, parameters) > 0;
				}
			});
		}

		public async Task LogoutAsync(Int32 userId)
		{
			Parameter.Validate(userId);

			await Task.Run(() =>
			{
				using (var mapper = new EntityMapper())
				{
					try
					{
						mapper.OpenTransaction();
						#region 设置用户下线
						{
							var user = new User();
							user.Offline();
							var result = mapper.Modify(user, acc => acc.Id == userId && !acc.IsDeleted && !acc.IsDisable);
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
							var result = mapper.Modify(online, on => on.UserId == userId && !on.IsDeleted);
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
				using (var mapper = new EntityMapper())
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
							var result = mapper.Modify(user, acc => acc.Id == userId);
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
				using (var mapper = new EntityMapper())
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
								var result = mapper.Modify(user, acc => acc.Id == user.Id);
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
							var result = mapper.Modify(userRole, acc => acc.UserId == user.Id);
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
							var result2 = mapper.Modify(user, ac => ac.Id == user.Id);
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
				using (var mapper = new EntityMapper())
				{
					var user = new User().Enable();
					var result = mapper.Modify(user, acc => acc.Id == userId);
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
				using (var mapper = new EntityMapper())
				{
					var parameters = new List<EntityParameter> { new EntityParameter("@userId", userId) };
					#region 前置条件验证
					{
						var result = mapper
						.Select<Role>().InnerJoin<UserRole>((a, b) => a.Id == b.RoleId).Where<UserRole>((a, b) => a.IsAllowDisable && b.UserId == userId).Count();

						//var sql = $@"SELECT COUNT(*) FROM Role AS a INNER JOIN UserRole AS a1 ON a1.UserId=@userId AND a1.RoleId=a.Id AND a1.IsDeleted=0
						//			 WHERE a.IsDeleted=0 AND a.IsAllowDisable=0";
						//var result = mapper.FindSingleValue<Int32>(sql, parameters);
						if (result > 0)
						{
							throw new BusinessException("当前用户拥有管理员角色，因此不能禁用或删除");
						}
					}
					#endregion
					{
						var user = new User().Disable();
						var result = mapper.Modify(user, acc => acc.Id == userId);
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
				using (var mapper = new EntityMapper())
				{
					var config = new Config().ModifyUserFace(newFace);
					var result = mapper.Modify(config, conf => conf.UserId == userId);
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
				using (var mapper = new EntityMapper())
				{
					var user = new User();
					if (isTogetherSetLockPassword)
					{
						user.ModifyLockScreenPassword(newPassword);
					}
					user.ModifyLoginPassword(newPassword);
					var result = mapper.Modify(user, acc => acc.Id == userId && acc.IsDisable == false);
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
				using (var mapper = new EntityMapper())
				{
					var user = new User().ModifyLockScreenPassword(newScreenPassword);
					var result = mapper.Modify(user, acc => acc.Id == userId && !acc.IsDisable);
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
				using (var mapper = new EntityMapper())
				{
					mapper.OpenTransaction();

					try
					{
						#region 前置条件验证
						{
							var isAdmin = mapper.Select<User>(a => new { a.IsAdmin }).Where(a => a.Id == userId && !a.IsDisable).FirstOrDefault().IsAdmin;
							//var sql = $@"SELECT a.IsAdmin FROM User AS a WHERE a.Id=@userId AND a.IsDeleted=0 AND a.IsDisable=0";
							//var parameters = new List<EntityParameter>
							//{
							//	new EntityParameter("@userId",userId)
							//};
							//var isAdmin = mapper.FindSingleValue<Boolean>(sql, parameters);
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
							var result = mapper.Modify(user, acc => acc.Id == userId && !acc.IsDisable);
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
							var result = mapper.Modify(config, conf => conf.UserId == userId);
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
							var result = mapper.Modify(userRole, accRole => accRole.UserId == userId);
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
							var result = mapper.Modify(member, mem => mem.UserId == userId);
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
