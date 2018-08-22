using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;

namespace NewCrmCore.Application.Services.Interface
{
    public interface IDeskServices
    {
        #region have return value

        /// <summary>
        /// 获取桌面的成员
        /// </summary>
        /// <returns></returns>
        Task<IDictionary<String, IList<dynamic>>> GetDeskMembersAsync(Int32 accountId);

        /// <summary>
        /// 根据用户id获取桌面的成员
        /// </summary>
        Task<MemberDto> GetMemberAsync(Int32 accountId, Int32 memberId, Boolean isFolder = default(Boolean));

        /// <summary>
        /// 检查成员名称
        /// </summary>
        Task<Boolean> CheckMemberNameAsync(String name);

        /// <summary>
        /// 获取未读通知数量
        /// </summary>
        Task<Int32> CheckUnreadNotifyCount(Int32 accountId);

        #endregion

        #region not have return value

        /// <summary>
        /// 修改默认显示的桌面
        /// </summary>
        Task ModifyDefaultDeskNumberAsync(Int32 accountId, Int32 newDefaultDeskNumber);

        /// <summary>
        /// 修改码头的位置
        /// </summary>
        Task ModifyDockPositionAsync(Int32 accountId, Int32 defaultDeskNumber, String newPosition);

        /// <summary>
        /// 桌面成员移动到码头中
        /// </summary>
        Task MemberInDockAsync(Int32 accountId, Int32 memberId);

        /// <summary>
        /// 桌面成员移出码头
        /// </summary>
        Task MemberOutDockAsync(Int32 accountId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 成员从码头移动到文件夹中
        /// </summary>
        Task DockToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从文件夹中移动到码头中
        /// </summary>
        Task FolderToDockAsync(Int32 accountId, Int32 memberId);

        /// <summary>
        /// 成员从桌面中移动到文件夹
        /// </summary>
        Task DeskToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从文件夹移动到桌面
        /// </summary>
        Task FolderToDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 成员从文件夹移动到另一个文件夹
        /// </summary>
        Task FolderToOtherFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从桌面移动到另一个桌面
        /// </summary>
        Task DeskToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 修改文件夹信息
        /// </summary>
        Task ModifyFolderInfoAsync(Int32 accountId, String memberName, String memberIcon, Int32 memberId);

        /// <summary>
        /// 卸载用户的桌面app成员
        /// </summary>
        Task UninstallMemberAsync(Int32 accountId, Int32 memberId);

        /// <summary>
        /// 修改成员信息
        /// </summary>
        Task ModifyMemberInfoAsync(Int32 accountId, MemberDto member);

        /// <summary>
        /// 创建新的文件夹
        /// </summary>
        Task CreateNewFolderAsync(String folderName, String folderImg, Int32 deskId, Int32 accountId);

        /// <summary>
        /// 从码头移动到另一个桌面
        /// </summary>
        Task DockToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 更新桌面成员的图标
        /// </summary>
        Task ModifyMemberIconAsync(Int32 accountId, Int32 memberId, String newIcon);

        /// <summary>
        /// 更新壁纸来源
        /// </summary>
        Task ModifyWallpaperSourceAsync(String source, Int32 accountId);

        #endregion
    }
}
