using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
    public interface IDeskContext
    {
        /// <summary>
        /// 修改成员排列方向X轴
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ModifyMemberDirectionToXAsync(Int32 userId);

        /// <summary>
        /// 修改成员排列方向X轴
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ModifyMemberDirectionToYAsync(Int32 userId);

        /// <summary>
        /// 修改成员图标显示大小
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        Task ModifyMemberDisplayIconSizeAsync(Int32 userId, Int32 newSize);

        /// <summary>
        /// 修改成员之间的垂直间距
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        Task ModifyMemberVerticalSpacingAsync(Int32 userId, Int32 newSize);

        /// <summary>
        /// 修改成员之间的水平间距
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        Task ModifyMemberHorizontalSpacingAsync(Int32 userId, Int32 newSize);

        /// <summary>
        /// 修改默认显示的桌面编号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newDefaultDeskNumber"></param>
        /// <returns></returns>
        Task ModifyDefaultDeskNumberAsync(Int32 userId, Int32 newDefaultDeskNumber);

        /// <summary>
        /// 修改码头位置
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
        /// 桌面成员移出码头中
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
        /// 成员从文件夹中移动到码头
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
        /// 成员从码头移动到桌面
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <param name="deskId"></param>
        /// <returns></returns>
        Task DockToOtherDeskAsync(Int32 userId, Int32 memberId, Int32 deskId);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="deskId"></param>
        /// <param name="folderName"></param>
        /// <param name="folderImg"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task CreateNewFolderAsync(Int32 deskId, String folderName, String folderImg, Int32 userId);

        /// <summary>
        /// 修改壁纸来源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ModifyWallpaperSourceAsync(String source, Int32 userId);

        /// <summary>
        /// 获取未读通知数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<Notify> CheckUnreadNotifyCount(Int32 userId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 读取通知
        /// </summary>
        /// <param name="notifyIds"></param>
        /// <returns></returns>
        Task ReadNotify(IList<Int32> notifyIds);
    }
}
