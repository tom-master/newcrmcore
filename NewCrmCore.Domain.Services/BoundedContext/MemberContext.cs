using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewLibCore;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCRM.Domain.Services.BoundedContext
{
	public class MemberContext: IMemberContext
	{
		public async Task<List<Member>> GetMembersAsync(Int32 accountId)
		{
			new Parameter().Validate(accountId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					var sql = $@"SELECT 
                            a.MemberType,
                            a.Id,
                            a.AppId,
                            a.Name,
                            a.IconUrl,
                            a.Width,
                            a.Height,
                            a.IsOnDock,
                            a.IsDraw,
                            a.IsOpenMax,
                            a.IsSetbar,
                            a.DeskIndex,
                            a.FolderId,
                            a.IsIconByUpload
                            FROM dbo.Member AS a WHERE a.AccountId=@AccountId AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@AccountId",accountId)
					};
					return dataStore.Find<Member>(sql, parameters);
				}
			});
		}

		public async Task<Member> GetMemberAsync(Int32 accountId, Int32 memberId, Boolean isFolder)
		{
			new Parameter().Validate(accountId).Validate(memberId);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					var where = new StringBuilder();
					var parameters = new List<SqlParameter>();
					if (isFolder)
					{
						parameters.Add(new SqlParameter("@Id", memberId));
						parameters.Add(new SqlParameter("@MemberType", (Int32)MemberType.Folder));
						where.Append($@" AND a.Id=@Id AND a.MemberType=@MemberType");
					}
					else
					{
						parameters.Add(new SqlParameter("@Id", memberId));
						where.Append($@" AND a.AppId=@Id");
					}

					var sql = $@"SELECT 
                    a.MemberType,
                    a.AppId,
                    a.AppUrl,
                    a.DeskIndex,
                    a.FolderId,
                    a.Height,
                    a.IconUrl,
                    a.Id,
                    a.IsDraw,
                    a.IsFlash,
                    a.IsFull,
                    a.IsLock,
                    a.IsMax,
                    a.IsOnDock,
                    a.IsOpenMax,
                    a.IsResize,
                    a.IsSetbar,
                    a.Name,
                    a.Width,
                    a.AccountId,
                    a.IsIconByUpload
                    FROM dbo.Member AS a WHERE a.AccountId=@AccountId {where} AND a.IsDeleted=0";
					parameters.Add(new SqlParameter("@AccountId", accountId));
					return dataStore.FindOne<Member>(sql, parameters);
				}
			});
		}

		public async Task<Boolean> CheckMemberNameAsync(String name)
		{
			new Parameter().Validate(name);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					var sql = $@"SELECT COUNT(*) FROM dbo.Member AS a WHERE a.Name=@name AND a.IsDeleted=0";
					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@name",name)
					};

					return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
				}
			});
		}

		public async Task ModifyFolderInfoAsync(Int32 accountId, String memberName, String memberIcon, Int32 memberId)
		{
			new Parameter().Validate(accountId).Validate(memberName).Validate(memberIcon).Validate(memberId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					#region sql
					{
						var member = new Member();
						member.ModifyName(memberName);
						member.ModifyIconUrl(memberIcon);
						dataStore.ExecuteModify(member, mem => mem.AccountId == accountId && mem.Id == memberId);
					}
					#endregion
				}
			});
		}

		public async Task ModifyMemberIconAsync(Int32 accountId, Int32 memberId, String newIcon)
		{
			new Parameter().Validate(accountId).Validate(memberId).Validate(newIcon);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					var member = new Member();
					member.ModifyIconUrl(newIcon);
					dataStore.ExecuteModify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
				}
			});
		}

		public async Task ModifyMemberInfoAsync(Int32 accountId, Member member)
		{
			new Parameter().Validate(accountId).Validate(member);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					if (member.IsIconByUpload)
					{
						member.IconFromUpload();
					}
					else
					{
						member.IconNotFromUpload();
					}

					member.ModifyIconUrl(member.IconUrl);
					member.ModifyName(member.Name);
					member.ModifyWidth(member.Width);
					member.ModifyHeight(member.Height);

					if (member.IsResize)
					{
						member.Resize();
					}
					else
					{
						member.NotResize();
					}

					if (member.IsOpenMax)
					{
						member.OpenMax();
					}
					else
					{
						member.NotOpenMax();
					}

					if (member.IsFlash)
					{
						member.Flash();
					}
					else
					{
						member.NotFlash();
					}


					dataStore.ExecuteModify(member, mem => mem.Id == member.Id && mem.AccountId == accountId);
				}
			});
		}

		public async Task UninstallMemberAsync(Int32 accountId, Int32 memberId)
		{
			new Parameter().Validate(accountId).Validate(memberId);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore(AppSettings.Get<Settings>().Database.Value))
				{
					dataStore.OpenTransaction();
					try
					{
						var isFolder = false;

						#region 判断是否为文件夹
						{
							var sql = $@"SELECT a.MemberType FROM dbo.Member AS a WHERE a.Id=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
							var parameters = new List<SqlParameter>
							{
								new SqlParameter("@Id", memberId),
								new SqlParameter("@AccountId", accountId)
							};
							isFolder = (dataStore.FindSingleValue<Int32>(sql, parameters)) == (Int32)MemberType.Folder;
						}
						#endregion

						if (isFolder)
						{
							#region 将文件夹内的成员移出
							{
								var member = new Member();
								member.ModifyFolderId(0);
								dataStore.ExecuteModify(member, mem => mem.AccountId == accountId && mem.FolderId == memberId);
							}
							#endregion
						}
						else
						{
							var appId = 0;

							#region 获取appId
							{
								var sql = $@"SELECT a.AppId FROM dbo.Member AS a WHERE a.Id=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
								var parameters = new List<SqlParameter>
								{
									new SqlParameter("@Id", memberId),
									new SqlParameter("@AccountId", accountId)
								};
								appId = dataStore.FindSingleValue<Int32>(sql, parameters);
							}
							#endregion
							App app = null;
							#region 查询app
							{
								var sql = $@"SELECT a.UseCount FROM dbo.App AS a WHERE a.Id=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
								var parameters = new List<SqlParameter>
								{
									new SqlParameter("@Id",appId),
									new SqlParameter("@AccountId",accountId)
								};
								app = dataStore.FindOne<App>(sql, parameters);
							}
							#endregion

							#region app使用量-1
							{
								app.DecreaseUseCount();
								dataStore.ExecuteModify(app, a => a.Id == appId && a.AccountId == accountId);
							}
							#endregion
						}

						#region 移除成员
						{
							var member = new Member();
							member.Remove();
							dataStore.ExecuteModify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
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
