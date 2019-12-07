using System;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Data.SQL.Mapper.Validate;

namespace NewCrmCore.Domain.Entitys.System
{
    /// <summary>
    /// 桌面应用
    /// </summary>
    [TableName("newcrm_user_member")]
    public partial class Member : EntityBase
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [Required]
        public Int32 AppId { get; private set; }

        /// <summary>
        /// 桌面应用的宽
        /// </summary>
        [Required]
        public Int32 Width { get; private set; }

        /// <summary>
        /// 桌面应用的高
        /// </summary>
        [Required]
        public Int32 Height { get; private set; }

        /// <summary>
        /// 文件夹Id
        /// </summary>
        [DefaultValue(typeof(Int32))]
        public Int32 FolderId { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required, InputRange(10)]
        public String Name { get; private set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        [Required, InputRange(150)]
        public String IconUrl { get; private set; }

        /// <summary>
        /// app地址
        /// </summary>
        [InputRange(150), DefaultValue(typeof(String))]
        public String AppUrl { get; private set; }

        /// <summary>
        /// 桌面应用是否在应用码头上
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsOnDock { get; private set; }

        /// <summary>
        /// 是否显示app底部的按钮
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsSetbar { get; private set; }

        /// <summary>
        /// 是否打开最大化
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsOpenMax { get; private set; }

        /// <summary>
        /// 是否为福莱希
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsFlash { get; private set; }

        /// <summary>
        /// 是否可以拉伸
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsResize { get; private set; }

        /// <summary>
        /// 桌面应用类型
        /// </summary>
        [Required]
        public MemberType MemberType { get; private set; }

        /// <summary>
        /// 桌面索引
        /// </summary>
        [DefaultValue(typeof(Int32), 1)]
        public Int32 DeskIndex { get; private set; }

        /// <summary>
        /// 账户Id
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// 图标是否来自上传
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsIconByUpload { get; private set; }

        public Double StarCount { get; set; }

        /// <summary>
        /// 实例化一个桌面应用对象
        /// </summary>
        public Member(
                    String name,
                    String iconUrl,
                    String appUrl,
                    Int32 appId,
                    Int32 width,
                    Int32 height,
                    Int32 userId,
                    Int32 deskIndex,
                    AppStyle appStyle,
                    Boolean isIconByUpload = default(Boolean),
                    Boolean isSetbar = default(Boolean),
                    Boolean isOpenMax = default(Boolean),
                    Boolean isFlash = default(Boolean),
                    Boolean isResize = default(Boolean))
        {
            AppId = appId;
            Width = width > 800 ? 800 : width;
            Height = height > 600 ? 600 : height;
            IsOpenMax = isOpenMax;
            IsSetbar = isSetbar;
            IsFlash = isFlash;
            IsResize = isResize;
            Name = name;
            IconUrl = iconUrl;
            AppUrl = appUrl;
            MemberType = appId == 0 ? MemberType.Folder : (appStyle == AppStyle.Widget ? MemberType.Widget : MemberType.App);
            DeskIndex = 1;
            IsIconByUpload = isIconByUpload;
            UserId = userId;
        }

        /// <summary>
        /// 实例化一个桌面应用对象
        /// </summary>
        public Member(String name,
            String iconUrl,
            Int32 appId,
            Int32 userId,
            Int32 deskIndex,
            Boolean isIconByUpload = default(Boolean))
        {
            AppId = appId;
            Width = 800;
            Height = 600;
            IsOpenMax = false;
            Name = name;
            IconUrl = iconUrl;
            DeskIndex = deskIndex;
            MemberType = appId == 0 ? MemberType.Folder : MemberType.App;
            IsIconByUpload = isIconByUpload;
            UserId = userId;
        }

        public Member()
        {
        }

    }

    public partial class Member
    {

        public Member ModifyWidth(Int32 width)
        {
            if (width <= 0)
            {
                throw new ArgumentException($@"{nameof(width)} 不能小于或等于0");
            }

            if (width == Width)
            {
                return this;
            }

            Width = width;
            OnChanged(nameof(Width));
            return this;
        }

        public Member ModifyHeight(Int32 height)
        {
            if (height <= 0)
            {
                throw new ArgumentException($@"{nameof(height)} 不能小于或等于0");
            }

            if (height == Height)
            {
                return this;
            }

            Height = height;
            OnChanged(nameof(Height));
            return this;
        }

        public Member ModifyFolderId(Int32 folderId)
        {
            if (folderId == FolderId)
            {
                return this;
            }

            FolderId = folderId;
            OnChanged(nameof(FolderId));
            return this;
        }

        public Member ModifyName(String name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException($@"{nameof(name)}不能为空");
            }

            if (name == Name)
            {
                return this;
            }

            Name = name;
            OnChanged(nameof(Name));
            return this;
        }

        public Member ModifyIconUrl(String iconUrl)
        {
            if (String.IsNullOrEmpty(iconUrl))
            {
                throw new ArgumentException($@"{nameof(iconUrl)}不能为空");
            }

            if (iconUrl == IconUrl)
            {
                return this;
            }

            IconUrl = iconUrl;
            OnChanged(nameof(IconUrl));
            return this;
        }

        public Member ModifyAppUrl(String appUrl)
        {
            if (String.IsNullOrEmpty(appUrl))
            {
                throw new ArgumentException($@"{nameof(appUrl)}不能为空");
            }

            if (appUrl == AppUrl)
            {
                return this;
            }

            AppUrl = appUrl;
            OnChanged(nameof(AppUrl));
            return this;
        }

        public Member OnDock()
        {
            IsOnDock = true;
            OnChanged(nameof(IsOnDock));
            return this;
        }

        public Member OutDock()
        {
            IsOnDock = false;
            OnChanged(nameof(IsOnDock));
            return this;
        }

        public Member Setbar()
        {
            IsSetbar = true;
            OnChanged(nameof(IsSetbar));
            return this;
        }

        public Member NotSetbar()
        {
            IsSetbar = false;
            OnChanged(nameof(IsSetbar));
            return this;
        }

        public Member OpenMax()
        {
            IsOpenMax = true;
            OnChanged(nameof(IsOpenMax));
            return this;
        }

        public Member NotOpenMax()
        {
            IsOpenMax = false;
            OnChanged(nameof(IsOpenMax));
            return this;
        }

        public Member Flash()
        {
            IsFlash = true;
            OnChanged(nameof(IsFlash));
            return this;
        }

        public Member NotFlash()
        {
            IsFlash = false;
            OnChanged(nameof(IsFlash));
            return this;
        }

        public Member Resize()
        {
            IsResize = true;
            OnChanged(nameof(IsResize));
            return this;
        }

        public Member NotResize()
        {
            IsResize = false;
            OnChanged(nameof(IsResize));
            return this;
        }

        public Member ModifyDeskIndex(Int32 deskIndex)
        {
            if (deskIndex <= 0)
            {
                throw new ArgumentException($@"{nameof(deskIndex)} 不能小于或等于0");
            }

            if (deskIndex == DeskIndex)
            {
                return this;
            }

            DeskIndex = deskIndex;
            OnChanged(nameof(DeskIndex));
            return this;
        }

        public Member IconNotFromUpload()
        {
            IsIconByUpload = false;
            OnChanged(nameof(IsIconByUpload));
            return this;
        }

        public Member IconFromUpload()
        {
            IsIconByUpload = true;
            OnChanged(nameof(IsIconByUpload));
            return this;
        }
    }
}
