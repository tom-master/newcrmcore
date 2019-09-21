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

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class MemberContext : IMemberContext
    {
        public async Task<List<Member>> GetMembersAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Query<Member>()
                    .Where(w => w.UserId == userId)
                    .Select(a => new
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
                    }).ToList();
                }
            });
        }

        public async Task<Member> GetMemberAsync(Int32 userId, Int32 memberId, Boolean isFolder)
        {
            Parameter.Validate(userId);
            Parameter.Validate(memberId);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
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
                    return mapper.SqlQuery(sql, parameters).FirstOrDefault<Member>();
                }
            });
        }

        public async Task<Boolean> CheckMemberNameAsync(String name)
        {
            Parameter.Validate(name);

            return await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    return mapper.Query<Member>().Where(w => w.Name == name).Count() > 0;
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
                using (var mapper = EntityMapper.CreateMapper())
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
                using (var mapper = EntityMapper.CreateMapper())
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
                using (var mapper = EntityMapper.CreateMapper())
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
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.OpenTransaction();
                    try
                    {
                        var isFolder = false;

                        #region 判断是否为文件夹
                        {

                            var member = mapper.Query<Member>()
                            .Where(w => w.Id == memberId && w.UserId == userId)
                            .Select(a => new
                            {
                                a.MemberType
                            }).FirstOrDefault();

                            isFolder = member.MemberType == MemberType.Folder;
                        }
                        #endregion

                        App app = null;
                        if (isFolder)
                        {
                            #region 判断文件夹内是否存在应用，如果存在则移出
                            {
                                var count = mapper.Query<Member>().Where(w => w.UserId == userId && w.FolderId == memberId).Count();
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
                                var member = mapper.Query<Member>()
                                .Where(w => w.Id == memberId && w.UserId == userId)
                                .Select(a => new
                                {
                                    a.AppId
                                }).FirstOrDefault();
                                appId = member.AppId;
                            }
                            #endregion

                            #region 查询应用
                            {
                                app = mapper.Query<App>()
                                .Where(w => w.Id == appId && w.UserId == userId)
                                .Select(a => new
                                {
                                    a.Name,
                                    a.UseCount,
                                    a.UserId
                                }).FirstOrDefault();
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
