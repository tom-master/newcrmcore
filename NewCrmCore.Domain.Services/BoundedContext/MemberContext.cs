using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Infrastructure;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;
using NewLibCore;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class MemberContext : IMemberContext
    {
        public async Task<List<Member>> GetMembersAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
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
                            a.IsOpenMax,
                            a.IsSetbar,
                            a.DeskIndex,
                            a.FolderId,
                            a.IsIconByUpload
                            FROM Member AS a WHERE a.AccountId=@AccountId AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@AccountId",accountId)
                    };
                    return dataStore.Find<Member>(sql, parameters);
                }
            });
        }

        public async Task<Member> GetMemberAsync(Int32 accountId, Int32 memberId, Boolean isFolder)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(memberId);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var where = new StringBuilder();
                    var parameters = new List<ParameterMapper>();
                    if (isFolder)
                    {
                        parameters.Add(new ParameterMapper("@Id", memberId));
                        parameters.Add(new ParameterMapper("@MemberType", MemberType.Folder.ToInt32()));
                        where.Append($@" AND a.Id=@Id AND a.MemberType=@MemberType");
                    }
                    else
                    {
                        parameters.Add(new ParameterMapper("@Id", memberId));
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
								a.IsFlash,
								a.IsOnDock,
								a.IsOpenMax,
								a.IsResize,
								a.IsSetbar,
								a.Name,
								a.Width,
								a.AccountId,
								a.IsIconByUpload,
								IFNULL((
									SELECT AVG(stars.StartNum) FROM AppStar AS stars WHERE stars.AppId=a.AppId AND stars.IsDeleted=0 GROUP BY stars.AppId
								),0) AS StarCount
								FROM Member AS a WHERE a.AccountId=@AccountId {where} AND a.IsDeleted=0";
                    parameters.Add(new ParameterMapper("@AccountId", accountId));
                    return dataStore.FindOne<Member>(sql, parameters);
                }
            });
        }

        public async Task<Boolean> CheckMemberNameAsync(String name)
        {
            Parameter.Validate(name);
            return await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var sql = $@"SELECT COUNT(*) FROM Member AS a WHERE a.Name=@name AND a.IsDeleted=0";
                    var parameters = new List<ParameterMapper>
                    {
                        new ParameterMapper("@name",name)
                    };

                    return dataStore.FindSingleValue<Int32>(sql, parameters) > 0;
                }
            });
        }


        public async Task ModifyFolderInfoAsync(Int32 accountId, String memberName, String memberIcon, Int32 memberId)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(memberName);
            Parameter.Validate(memberIcon);
            Parameter.Validate(memberId);

            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    #region sql
                    {
                        var member = new Member();
                        member.ModifyName(memberName);
                        member.ModifyIconUrl(memberIcon);
                        dataStore.Modify(member, mem => mem.AccountId == accountId && mem.Id == memberId);
                    }
                    #endregion
                }
            });
        }

        public async Task ModifyMemberIconAsync(Int32 accountId, Int32 memberId, String newIcon)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(memberId);
            Parameter.Validate(newIcon);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    var member = new Member();
                    member.ModifyIconUrl(newIcon);
                    dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
                }
            });
        }

        public async Task ModifyMemberInfoAsync(Int32 accountId, Member member)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(member);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
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


                    dataStore.Modify(member, mem => mem.Id == member.Id && mem.AccountId == accountId);
                }
            });
        }

        public async Task UninstallMemberAsync(Int32 accountId, Int32 memberId)
        {
            Parameter.Validate(accountId);
            Parameter.Validate(memberId);
            await Task.Run(() =>
            {
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.OpenTransaction();
                    try
                    {
                        var isFolder = false;

                        #region 判断是否为文件夹
                        {
                            var sql = $@"SELECT a.MemberType FROM Member AS a WHERE a.Id=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
                            var parameters = new List<ParameterMapper>
                            {
                                new ParameterMapper("@Id", memberId),
                                new ParameterMapper("@AccountId", accountId)
                            };
                            isFolder = (dataStore.FindSingleValue<Int32>(sql, parameters)) == MemberType.Folder.ToInt32();
                        }
                        #endregion

                        if (isFolder)
                        {
                            #region 将文件夹内的成员移出
                            {
                                var member = new Member();
                                member.ModifyFolderId(0);
                                dataStore.Modify(member, mem => mem.AccountId == accountId && mem.FolderId == memberId);
                            }
                            #endregion
                        }
                        else
                        {
                            var appId = 0;

                            #region 获取appId
                            {
                                var sql = $@"SELECT a.AppId FROM Member AS a WHERE a.Id=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
                                var parameters = new List<ParameterMapper>
                                {
                                    new ParameterMapper("@Id", memberId),
                                    new ParameterMapper("@AccountId", accountId)
                                };
                                appId = dataStore.FindSingleValue<Int32>(sql, parameters);
                            }
                            #endregion

                            App app = null;

                            #region 查询app
                            {
                                var sql = $@"SELECT a.UseCount FROM App AS a WHERE a.Id=@Id AND a.AccountId=@AccountId AND a.IsDeleted=0";
                                var parameters = new List<ParameterMapper>
                                {
                                    new ParameterMapper("@Id",appId),
                                    new ParameterMapper("@AccountId",accountId)
                                };
                                app = dataStore.FindOne<App>(sql, parameters);
                            }
                            #endregion

                            #region app使用量-1
                            {
                                app.DecreaseUseCount();
                                dataStore.Modify(app, a => a.Id == appId && a.AccountId == accountId);
                            }
                            #endregion
                        }

                        #region 移除成员
                        {
                            var member = new Member();
                            member.Remove();
                            dataStore.Modify(member, mem => mem.Id == memberId && mem.AccountId == accountId);
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
