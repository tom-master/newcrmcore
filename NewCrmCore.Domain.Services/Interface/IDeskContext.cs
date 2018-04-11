using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCRM.Domain.Services.Interface
{
    public interface IDeskContext
    {
        /// <summary>
        /// 修改成员排列方向X轴
        /// </summary>
        Task ModifyMemberDirectionToXAsync(Int32 accountId);

        /// <summary>
        /// 修改成员排列方向X轴
        /// </summary>
        Task ModifyMemberDirectionToYAsync(Int32 accountId);

        /// <summary>
        /// 修改成员图标显示大小
        /// </summary>
        Task ModifyMemberDisplayIconSizeAsync(Int32 accountId, Int32 newSize);

        /// <summary>
        /// 修改成员之间的垂直间距
        /// </summary>
        Task ModifyMemberVerticalSpacingAsync(Int32 accountId, Int32 newSize);

        /// <summary>
        /// 修改成员之间的水平间距
        /// </summary>
        Task ModifyMemberHorizontalSpacingAsync(Int32 accountId, Int32 newSize);

        /// <summary>
        /// 修改默认显示的桌面编号
        /// </summary>
        Task ModifyDefaultDeskNumberAsync(Int32 accountId, Int32 newDefaultDeskNumber);

        /// <summary>
        /// 修改码头位置
        /// </summary>
        Task ModifyDockPositionAsync(Int32 accountId, Int32 defaultDeskNumber, String newPosition);

        /// <summary>
	    /// 桌面成员移动到码头中
	    /// </summary>
	    Task MemberInDockAsync(Int32 accountId, Int32 memberId);

        /// <summary>
        /// 桌面成员移出码头中
        /// </summary>
        Task MemberOutDockAsync(Int32 accountId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 成员从码头移动到文件夹中
        /// </summary>
        Task DockToFolderAsync(Int32 accountId, Int32 memberId, Int32 folderId);

        /// <summary>
        /// 成员从文件夹中移动到码头
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
        /// 成员从码头移动到桌面
        /// </summary>
        Task DockToOtherDeskAsync(Int32 accountId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        Task CreateNewFolderAsync(Int32 deskId, String folderName, String folderImg, Int32 accountId);

        /// <summary>
        /// 修改壁纸来源
        /// </summary>
        Task ModifyWallpaperSourceAsync(String source, Int32 accountId);
    }
}
