using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject; 
using NewLibCore.Validate;
using NewLibCore;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.SQL.Mapper;
using System.Linq;

namespace NewCrmCore.Domain.Services.BoundedContext
{
	public class MemberContext : IMemberContext
	{
		public async Task<List<Member>> GetMembersAsync(Int32 userId)
		{
			Parameter.Validate(userId);

			return await Task.Run(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{
					return mapper.Select<Member>(a => new
					{
						a.MemberType,
						a.Id,
						a.AppId,
						a.Name,
						a.IconUrl,
						a.Width,
						a.Height,
						a.IsOnDock,
						a.IsOpenMax,
						a.IsSetbar,
						a.DeskIndex,
						a.FolderId,
						a.IsIconByUpload
					}).Where(w => w.UserId == userId).ToList();

					//return mapper.Find<Member>(a => a.UserId == userId, a => new
					//{
					//    a.MemberType,
					//    a.Id,
					//    a.AppId,
					//    a.Name,
					//    a.IconUrl,
					//    a.Width,
					//    a.Height,
					//    a.IsOnDock,
					//    a.IsOpenMax,
					//    a.IsSetbar,
					//    a.DeskIndex,
					//    a.FolderId,
					//    a.IsIconByUpload
					//}).ToList();


					//var sql = $@"SELECT 
					//        a.MemberType,
					//        a.Id,
					//        a.AppId,
					//        a.Name,
					//        a.IconUrl,
					//        a.Width,
					//        a.Height,
					//        a.IsOnDock,
					//        a.IsOpenMax,
					//        a.IsSetbar,
					//        a.DeskIndex,
					//        a.FolderId,
					//        a.IsIconByUpload
					//        FROM Member AS a WHERE a.UserId=@UserId AND a.IsDeleted=0";
					//var parameters = new List<EntityParameter>
					//{
					//    new EntityParameter("@UserId",userId)
					//};
					//return dataStore.Find<Member>(sql, parameters);
				}
			});
		}

		public async Task<Member> GetMemberAsync(Int32 userId, Int32 memberId, Boolean isFolder)
		{
			Parameter.Validate(userId);
			Parameter.Validate(memberId);

			return await Task.Run(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{
					var where = new StringBuilder();
					var parameters = new List<EntityParameter>();
					if (isFolder)
					{
						parameters.Add(new EntityParameter("@Id", memberId));
						parameters.Add(new EntityParameter("@MemberType", MemberType.Folder.ToInt32()));
						where.Append($@" AND a.Id=@Id AND a.MemberType=@MemberType");
					}
					else
					{
						parameters.Add(new EntityParameter("@Id", memberId));
						where.Append($@" AND a.Id=@Id OR a.AppId=@Id");
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
								a.IsFlash,
								a.IsOnDock,
								a.IsOpenMax,
								a.IsResize,
								a.IsSetbar,
								a.Name,
								a.Width,
								a.UserId,
								a.IsIconByUpload,
								IFNULL((
									SELECT AVG(stars.StartNum) FROM newcrm_app_star AS stars WHERE stars.AppId=a.AppId AND stars.IsDeleted=0 GROUP BY stars.AppId
								),0) AS StarCount
								FROM newcrm_user_member AS a WHERE a.UserId=@UserId {where} AND a.IsDeleted=0";
					parameters.Add(new EntityParameter("@UserId", userId));
					return mapper.ExecuteToSingle<Member>(sql, parameters);
				}
			});
		}

		public async Task<Boolean> CheckMemberNameAsync(String name)
		{
			Parameter.Validate(name);

			return await Task.Run(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{
					return mapper.Select<Member>().Where(w => w.Name == name).Exist();

					//return mapper.Count<Member>(a => a.Name == name) > 0;
					//var sql = $@"SELECT COUNT(*) FROM Member AS a WHERE a.Name=@name AND a.IsDeleted=0";
					//var parameters = new List<EntityParameter>
					//{
					//    new EntityParameter("@name",name)
					//};

					//return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
				}
			});
		}


		public async Task ModifyFolderInfoAsync(Int32 userId, String memberName, String memberIcon, Int32 memberId)
		{
			Parameter.Validate(userId);
			Parameter.Validate(memberName);
			Parameter.Validate(memberIcon);
			Parameter.Validate(memberId);

			await Task.Run(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{
					var member = new Member();
					member.ModifyName(memberName);
					member.ModifyIconUrl(memberIcon);
					var result = mapper.Update(member, mem => mem.UserId == userId && mem.Id == memberId);
					if (!result)
					{
						throw new BusinessException("修改文件夹信息失败");
					}
				}
			});
		}

		public async Task ModifyMemberIconAsync(Int32 userId, Int32 memberId, String newIcon)
		{
			Parameter.Validate(userId);
			Parameter.Validate(memberId);
			Parameter.Validate(newIcon);
			await Task.Run(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{
					var member = new Member();
					member.ModifyIconUrl(newIcon);
					var result = mapper.Update(member, mem => mem.Id == memberId && mem.UserId == userId);
					if (!result)
					{
						throw new BusinessException("修改桌面应用图片失败");
					}
				}
			});
		}

		public async Task ModifyMemberInfoAsync(Int32 userId, Member member)
		{
			Parameter.Validate(userId);
			Parameter.Validate(member);
			await Task.Run(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{ 
					var result = mapper.Update(member, mem => mem.Id == member.Id && mem.UserId == userId);
					if (!result)
					{
						throw new BusinessException("修改桌面应用信息失败");
					}
				}
			});
		}

		public async Task<App> UninstallMemberAsync(Int32 userId, Int32 memberId)
		{
			Parameter.Validate(userId);
			Parameter.Validate(memberId);
			return await Task.Run<App>(() =>
			{
				using(var mapper = EntityMapper.CreateMapper())
				{
					mapper.OpenTransaction();
					try
					{
						var isFolder = false;
						App app = null;
						#region 判断是否为文件夹
						{

							var member = mapper.Select<Member>(a => new { a.MemberType }).Where(w => w.Id == memberId && w.UserId == userId).FirstOrDefault();

							//var sql = $@"SELECT a.MemberType FROM Member AS a WHERE a.Id=@Id AND a.UserId=@UserId AND a.IsDeleted=0";
							//var parameters = new List<EntityParameter>
							//{
							//	new EntityParameter("@Id", memberId),
							//	new EntityParameter("@UserId", userId)
							//};
							isFolder = member.MemberType == MemberType.Folder;
						}
						#endregion

						if (isFolder)
						{
							#region 判断文件夹内是否存在应用，如果存在则移出
							{
								var count = mapper.Select<Member>().Where(w => w.UserId == userId && w.FolderId == memberId).Count();
								//var sql = $@"SELECT COUNT(*) FROM Member AS a WHERE a.IsDeleted=0 AND a.UserId=@userId AND a.FolderId=@folderId";
								//var parameters = new List<EntityParameter>
								//{
								//	new EntityParameter("@userId",userId),
								//	new EntityParameter("@folderId",memberId)
								//};
								//var count = mapper.ExecuteSql<Int32>(sql, parameters);
								if (count > 0)
								{
									var member = new Member();
									member.ModifyFolderId(0);
									var result = mapper.Update(member, mem => mem.UserId == userId && mem.FolderId == memberId);
									if (!result)
									{
										throw new BusinessException("将文件夹内的桌面应用移出失败");
									}
								}
							}
							#endregion
						}
						else
						{
							var appId = 0;

							#region 获取appId
							{
								var member = mapper.Select<Member>(a => new { a.AppId }).Where(w => w.Id == memberId && w.UserId == userId).FirstOrDefault();
								//var sql = $@"SELECT a.AppId FROM Member AS a WHERE a.Id=@Id AND a.UserId=@UserId AND a.IsDeleted=0";
								//var parameters = new List<EntityParameter>
								//{
								//	new EntityParameter("@Id", memberId),
								//	new EntityParameter("@UserId", userId)
								//};
								appId = member.AppId;
							}
							#endregion

							#region 查询应用
							{

								app = mapper.Select<App>(a => new { a.Name, a.UseCount, a.UserId }).Where(w => w.Id == appId && w.UserId == userId).FirstOrDefault();
								//var sql = $@"SELECT a.Name,a.UseCount,a.UserId FROM App AS a WHERE a.Id=@Id AND a.IsDeleted=0";
								//var parameters = new List<EntityParameter>
								//{
								//	new EntityParameter("@Id",appId),
								//	new EntityParameter("@UserId",userId)
								//};
								//app = mapper.ComplexSqlExecute<App>(sql, parameters);
							}
							#endregion

							#region 应用使用量-1
							{
								app.DecreaseUseCount();
								var result = mapper.Update(app, a => a.Id == appId && a.UserId == app.UserId);
								if (!result)
								{
									throw new BusinessException("修改应用使用数量失败");
								}
							}
							#endregion
						}

						#region 移除桌面应用
						{
							var member = new Member();
							member.Remove();
							var result = mapper.Update(member, mem => mem.Id == memberId && mem.UserId == userId);
							if (!result)
							{
								throw new BusinessException("移除桌面应用失败");
							}
						}
						#endregion

						mapper.Commit();
						return app;
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
