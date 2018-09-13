using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;

namespace NewCrmCore.Domain.Services.Interface
{
    public interface IMemberContext
    {
        /// <summary>
        /// 获取桌面应用列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Member>> GetMembersAsync(Int32 userId);

        /// <summary>
        /// 获取桌面应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        Task<Member> GetMemberAsync(Int32 userId, Int32 memberId, Boolean isFolder);

        /// <summary>
        /// 检查桌面应用名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Boolean> CheckMemberNameAsync(String name);

        /// <summary>
        /// 修改文件夹的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberName"></param>
        /// <param name="memberIcon"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task ModifyFolderInfoAsync(Int32 userId, String memberName, String memberIcon, Int32 memberId);

        /// <summary>
        /// 修改桌面应用信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        Task ModifyMemberInfoAsync(Int32 userId, Member member);

        /// <summary>
        /// 卸载用户的桌面应用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task UninstallMemberAsync(Int32 userId, Int32 memberId);

        /// <summary>
        /// 修改桌面应用的Icon
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="newIcon"></param>
        /// <returns></returns>
        Task ModifyMemberIconAsync(Int32 userId, Int32 memberId, String newIcon);
    }
}
