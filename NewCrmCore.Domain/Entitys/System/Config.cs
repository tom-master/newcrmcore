using System;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.SQL.Mapper.Extension;
using NewLibCore.Data.SQL.Mapper.Extension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    public partial class Config : EntityBase
    {
        /// <summary>
        /// 皮肤
        /// </summary>
        [Required, InputRange(10)]
        public String Skin { get; private set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [Required, InputRange(150)]
        public String UserFace { get; private set; }

        /// <summary>
        /// app尺寸
        /// </summary>
        [Required]
        public Int32 AppSize { get; private set; }

        /// <summary>
        /// app垂直间距
        /// </summary>
        [Required]
        public Int32 AppVerticalSpacing { get; private set; }

        /// <summary>
        /// app水平间距
        /// </summary>
        [Required]
        public Int32 AppHorizontalSpacing { get; private set; }

        /// <summary>
        /// 默认桌面编号
        /// </summary>
        [DefaultValue(typeof(Int32), 1)]
        public Int32 DefaultDeskNumber { get; private set; }

        /// <summary>
        /// 默认桌面数量
        /// </summary>
        [DefaultValue(typeof(Int32), 5)]
        public Int32 DefaultDeskCount { get; private set; }

        /// <summary>
        /// 壁纸的展示模式
        /// </summary>
        [Required]
        public WallpaperMode WallpaperMode { get; private set; }

        /// <summary>
        /// 壁纸来源
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsBing { get; private set; }

        /// <summary>
        /// app排列方向
        /// </summary>
        [Required]
        public AppAlignMode AppXy { get; private set; }

        /// <summary>
        /// 码头位置
        /// </summary>
        [DefaultValue(typeof(DockPosition), DockPosition.Top)]
        public DockPosition DockPosition { get; private set; }

        /// <summary>
        /// 账户Id
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// 壁纸Id
        /// </summary>
        [Required]
        public Int32 WallpaperId { get; private set; }

        /// <summary>
        /// 账户头像是否被更改
        /// </summary>
        [Required]
        public Boolean IsModifyUserFace { get; private set; }

        public Config(Int32 userId)
        {
            AppXy = AppAlignMode.X;
            DockPosition = DockPosition.Top;
            UserFace = @"/images/ui/avatar_48.jpg";
            Skin = "default";
            WallpaperMode = WallpaperMode.Fill;
            AppSize = 48;
            AppVerticalSpacing = 50;
            AppHorizontalSpacing = 50;
            DefaultDeskNumber = 1;
            DefaultDeskCount = 5;
            UserId = userId;
            IsBing = true;
            IsModifyUserFace = false;
            WallpaperId = 3;
        }

        public Config() { }
    }

    public partial class Config
    {
        public Config ModifyUserId(Int32 userId)
        {
            if (userId == UserId)
            {
                return this;
            }

            UserId = userId;
            OnPropertyChanged(nameof(UserId), UserId);
            return this;
        }

        public Config ModifySkin(String skin)
        {
            if (String.IsNullOrEmpty(skin))
            {
                throw new ArgumentException($@"{nameof(skin)} 不能为空");
            }

            if (skin == Skin)
            {
                return this;
            }

            Skin = skin;
            OnPropertyChanged(nameof(Skin), Skin);
            return this;
        }

        public Config ModifyUserFace(String userFace)
        {
            if (String.IsNullOrEmpty(userFace))
            {
                throw new ArgumentException($@"{nameof(userFace)} 不能为空");
            }

            if (userFace == UserFace)
            {
                return this;
            }

            UserFace = userFace;
            OnPropertyChanged(nameof(UserFace), UserFace);

            IsModifyUserFace = true;
            OnPropertyChanged(nameof(IsModifyUserFace), IsModifyUserFace);

            return this;
        }

        public Config ModifyAppSize(Int32 appSize)
        {
            if (appSize <= 0)
            {
                throw new ArgumentException($@"{nameof(appSize)} 不能小于或者等于0");
            }

            if (appSize == AppSize)
            {
                return this;
            }

            if (appSize < 32)
            {
                appSize = 32;
            }
            else if (appSize > 64)
            {
                appSize = 64;
            }

            AppSize = appSize;
            OnPropertyChanged(nameof(AppSize), AppSize);
            return this;
        }

        public Config ModifyAppVerticalSpacing(Int32 appVerticalSpacing)
        {
            if (appVerticalSpacing <= 0)
            {
                throw new ArgumentException($@"{nameof(appVerticalSpacing)} 不能小于或者等于0");
            }

            if (appVerticalSpacing == AppVerticalSpacing)
            {
                return this;
            }

            if (appVerticalSpacing < 0)
            {
                appVerticalSpacing = 0;
            }
            else if (appVerticalSpacing > 100)
            {
                appVerticalSpacing = 100;
            }

            AppVerticalSpacing = appVerticalSpacing;
            OnPropertyChanged(nameof(AppVerticalSpacing), AppVerticalSpacing);
            return this;
        }

        public Config ModifyAppHorizontalSpacing(Int32 appHorizontalSpacing)
        {
            if (appHorizontalSpacing <= 0)
            {
                throw new ArgumentException($@"{nameof(appHorizontalSpacing)} 不能小于或者等于0");
            }

            if (appHorizontalSpacing == AppHorizontalSpacing)
            {
                return this;
            }

            if (appHorizontalSpacing < 0)
            {
                appHorizontalSpacing = 0;
            }
            else if (appHorizontalSpacing > 100)
            {
                appHorizontalSpacing = 100;
            }

            AppHorizontalSpacing = appHorizontalSpacing;
            OnPropertyChanged(nameof(AppHorizontalSpacing), AppHorizontalSpacing);
            return this;
        }

        public Config ModifyDefaultDeskNumber(Int32 deskNumber)
        {
            if (deskNumber <= 0)
            {
                throw new ArgumentException($@"{nameof(deskNumber)} 不能小于或者等于0");
            }

            if (deskNumber == DefaultDeskNumber)
            {
                return this;
            }

            DefaultDeskNumber = deskNumber;
            OnPropertyChanged(nameof(DefaultDeskNumber), DefaultDeskNumber);
            return this;
        }

        public Config ModifyDefaultDeskCount(Int32 deskCount)
        {
            if (deskCount <= 0)
            {
                throw new ArgumentException($@"{nameof(deskCount)} 不能小于或者等于0");
            }

            if (deskCount == DefaultDeskCount)
            {
                return this;
            }

            DefaultDeskCount = deskCount;
            OnPropertyChanged(nameof(DefaultDeskCount), DefaultDeskCount);
            return this;
        }

        public Config ModeTo(WallpaperMode mode)
        {
            if (mode == WallpaperMode)
            {
                return this;
            }

            WallpaperMode = mode;
            OnPropertyChanged(nameof(WallpaperMode), WallpaperMode);
            return this;
        }

        public Config DisplayToTile()
        {
            WallpaperMode = WallpaperMode.Tile;
            OnPropertyChanged(nameof(WallpaperMode), WallpaperMode);
            return this;
        }

        public Config DisplayToDraw()
        {
            WallpaperMode = WallpaperMode.Draw;
            OnPropertyChanged(nameof(WallpaperMode), WallpaperMode);
            return this;
        }

        public Config DisplayToCenter()
        {
            WallpaperMode = WallpaperMode.Center;
            OnPropertyChanged(nameof(WallpaperMode), WallpaperMode);
            return this;
        }

        public Config FromBing()
        {
            IsBing = true;
            OnPropertyChanged(nameof(IsBing), IsBing);
            return this;
        }

        public Config NotFromBing()
        {
            IsBing = false;
            OnPropertyChanged(nameof(IsBing), IsBing);
            return this;
        }

        public Config DirectionToX()
        {
            AppXy = AppAlignMode.X;
            OnPropertyChanged(nameof(AppXy), AppXy);
            return this;
        }

        public Config DirectionToY()
        {
            AppXy = AppAlignMode.Y;
            OnPropertyChanged(nameof(AppXy), AppXy);
            return this;
        }

        public Config PositionTo(DockPosition postion)
        {
            if (postion == DockPosition)
            {
                return this;
            }

            DockPosition = postion;
            OnPropertyChanged(nameof(DockPosition), DockPosition);
            return this;
        }

        public Config ModifyWallpaperId(Int32 wallpaperId)
        {
            if (wallpaperId == WallpaperId)
            {
                return this;
            }

            WallpaperId = wallpaperId;
            OnPropertyChanged(nameof(WallpaperId), WallpaperId);
            return this;
        }
    }
}
