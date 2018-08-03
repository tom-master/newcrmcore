using System;
using System.ComponentModel.DataAnnotations;
using NewCrmCore.Domain.ValueObject;
namespace NewCrmCore.Dto
{
    public sealed class ConfigDto : BaseDto
    {
        /// <summary>
        /// 皮肤
        /// </summary>
        public String Skin { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public String AccountFace { get; set; }

        /// <summary>
        /// app尺寸
        /// </summary>
        public Int32 AppSize { get; set; }

        /// <summary>
        /// app垂直间距
        /// </summary>
        public Int32 AppVerticalSpacing { get; set; }

        /// <summary>
        /// app水平间距
        /// </summary>
        public Int32 AppHorizontalSpacing { get; set; }

        /// <summary>
        /// 默认桌面
        /// </summary>
        public Int32 DefaultDeskNumber { get; set; }

        /// <summary>
        /// app排列方向
        /// </summary>
        public AppAlignMode AppXy { get; set; }

        /// <summary>
        /// 码头位置
        /// </summary>
        public DockPosition DockPosition { get; set; }

        /// <summary>
        /// 壁纸
        /// </summary>
        public String WallpaperUrl { get; set; }

        /// <summary>
        /// 壁纸宽
        /// </summary>
        public Int32 WallpaperWidth { get; set; }

        /// <summary>
        ///壁纸高
        /// </summary>
        public Int32 WallpaperHeigth { get; set; }

        /// <summary>
        /// 壁纸来源
        /// </summary>
        public WallpaperSource WallpaperSource { get; set; }

        /// <summary>
        /// 壁纸的展示模式
        /// </summary>
        public WallpaperMode WallpaperMode { get; set; }

        /// <summary>
        /// 默认桌面数量
        /// </summary>
        public Int32 DefaultDeskCount { get; set; }

        /// <summary>
        /// 账户Id
        /// </summary>
        public Int32 AccountId { get; set; }

        /// <summary>
        /// 图片是否来自bing
        /// </summary>
        public Boolean IsBing { get; set; }

    }
}
