using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
    public interface IDeskServices
    {
        #region have return value

        /// <summary>
        /// 获取桌面的成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IDictionary<String, IList<dynamic>>> GetDeskMembersAsync(Int32 userId);

        /// <summary>
        /// 根据用户id获取桌面的成员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        Task<MemberDto> GetMemberAsync(Int32 userId, Int32 memberId, Boolean isFolder = default(Boolean));

        /// <summary>
        /// 检查成员名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Boolean> CheckMemberNameAsync(String name);

        /// <summary>
        /// 获取未读通知数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageList<NotifyDto>> CheckUnreadNotifyCount(Int32 userId, Int32 pageIndex, Int32 pageSize);

        /// <summary>
        /// 获取全部的皮肤
        /// </summary>
        /// <param name="skinPath"></param>
        /// <returns></returns>
        Task<IDictionary<String, dynamic>> GetAllSkinAsync(String skinPath);

        #endregion

        #region not have return value

        /// <summary>
        /// 修改默认显示的桌面
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newDefaultDeskNumber"></param>
        /// <returns></returns>
        Task ModifyDefaultDeskNumberAsync(Int32 userId, Int32 newDefaultDeskNumber);

        /// <summary>
        /// 修改码头的位置
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="defaultDeskNumber"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        Task ModifyDockPositionAsync(Int32 userId, Int32 defaultDeskNumber, String newPosition);

        /// <summary>
        /// 桌面成员移动到码头中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task MemberInDockAsync(Int32 userId, Int32 memberId);

        /// <summary>
        /// 桌面成员移出码头
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="deskId"></param>
        /// <returns></returns>
        Task MemberOutDockAsync(Int32 userId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 成员从码头移动到文件夹中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task DockToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从文件夹中移动到码头中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task FolderToDockAsync(Int32 userId, Int32 memberId);

        /// <summary>
        /// 成员从桌面中移动到文件夹
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task DeskToFolderAsync(Int32 userId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从文件夹移动到桌面
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="deskId"></param>
        /// <returns></returns>
        Task FolderToDeskAsync(Int32 userId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 成员从文件夹移动到另一个文件夹
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task FolderToOtherFolderAsync(Int32 userId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从桌面移动到另一个桌面
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="deskId"></param>
        /// <returns></returns>
        Task DeskToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 修改文件夹信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberName"></param>
        /// <param name="memberIcon"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task ModifyFolderInfoAsync(Int32 userId, String memberName, String memberIcon, Int32 memberId);

        /// <summary>
        /// 卸载用户的桌面成员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task UninstallMemberAsync(Int32 userId, Int32 memberId);

        /// <summary>
        /// 修改成员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        Task ModifyMemberInfoAsync(Int32 userId, MemberDto member);

        /// <summary>
        /// 创建新的文件夹
        /// </summary>
        Task CreateNewFolderAsync(String folderName, String folderImg, Int32 deskId, Int32 userId);

        /// <summary>
        /// 从码头移动到另一个桌面
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="deskId"></param>
        /// <returns></returns>
        Task DockToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 更新桌面成员的图标
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="newIcon"></param>
        /// <returns></returns>
        Task ModifyMemberIconAsync(Int32 userId, Int32 memberId, String newIcon);

        /// <summary>
        /// 更新壁纸来源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ModifyWallpaperSourceAsync(String source, Int32 userId);

        /// <summary>
        /// 读取通知
        /// </summary>
        /// <param name="notifyIds"></param>
        /// <returns></returns>
        Task ReadNotify(IList<Int32> notifyIds);

        /// <summary>
        /// 修改默认显示的皮肤
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSkin"></param>
        Task ModifySkinAsync(Int32 userId, String newSkin);

        #endregion
    }
}
