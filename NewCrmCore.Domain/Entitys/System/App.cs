using System;
using NewCrmCore.Domain.ValueObject; 
using NewLibCore.Data.SQL.Mapper.EntityExtension;
using NewLibCore.Data.SQL.Mapper.EntityExtension.EntityAttribute;

namespace NewCrmCore.Domain.Entitys.System
{
    [TableName("newcrm_app")]
    public partial class App : EntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required, InputRange(2, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        [Required, InputRange(150)]
        public String IconUrl { get; private set; }

        /// <summary>
        /// app地址
        /// </summary>
        [Required, InputRange(150)]
        public String AppUrl { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [InputRange(50), DefaultValue(typeof(String))]
        public String Remark { get; private set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [Required]
        public Int32 Width { get; private set; }

        /// <summary>
        /// 高度
        /// </summary>
        [Required]
        public Int32 Height { get; private set; }

        /// <summary>
        /// 使用数
        /// </summary>
        [DefaultValue(typeof(Int32))]
        public Int32 UseCount { get; private set; }

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
        /// 是否为系统应用
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsSystem { get; private set; }

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
        /// 开发者(用户)Id
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// App类型Id
        /// </summary>
        [Required]
        public Int32 AppTypeId { get; private set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsRecommand { get; private set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [DefaultValue(typeof(AppAuditState), AppAuditState.UnAuditState)]
        public AppAuditState AppAuditState { get; private set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        [DefaultValue(typeof(AppReleaseState), AppReleaseState.UnRelease)]
        public AppReleaseState AppReleaseState { get; private set; }

        /// <summary>
        /// app样式
        /// </summary>
        [Required]
        public AppStyle AppStyle { get; private set; }

        /// <summary>
        /// 是否安装
        /// </summary>
        public Boolean IsInstall { get; private set; }

        /// <summary>
        /// 评价
        /// </summary>
        /// <value></value>
        public Double StarCount { get; private set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public String UserName { get; private set; }

        /// <summary>
        /// 图标是否来自上传
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsIconByUpload { get; private set; }

        /// <summary>
        /// 实例化一个app对象
        /// </summary>
        public App(String name,
            String iconUrl,
            String appUrl,
            Int32 width,
            Int32 height,
            Int32 appTypeId,
            Boolean isResize,
            Boolean isOpenMax,
            Boolean isFlash,
            Boolean isSetbar,
            AppAuditState appAuditState,
            AppReleaseState appReleaseState,
            AppStyle appStyle = AppStyle.App,
            Int32 userId = default(Int32),
            String remark = default(String),
            Boolean isIconByUpload = default(Boolean))
        {
            Name = name;
            IconUrl = iconUrl;
            AppUrl = appUrl;
            Width = width > 800 ? 800 : width;
            Height = height > 600 ? 600 : height;
            AppTypeId = appTypeId;
            AppStyle = appStyle;
            if (userId == 0)
            {
                IsSystem = true;
            }
            else
            {
                IsSystem = false;
                UserId = userId;
            }

            Remark = remark;
            AppAuditState = appAuditState;
            AppReleaseState = appReleaseState;
            UseCount = 0;
            IsRecommand = false;
            IsIconByUpload = isIconByUpload;
        }

        public App() { }
    }

    /// <summary>
    /// AppExtension
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// 修改app名称
        /// </summary>
        public App ModifyName(String appName)
        {
            if (String.IsNullOrEmpty(appName))
            {
                throw new ArgumentException($@"{nameof(appName)} 不能为空");
            }

            if (appName == Name)
            {
                return this;
            }

            Name = appName;
            OnChanged(nameof(Name));
            return this;
        }

        /// <summary>
        /// 修改app图标
        /// </summary>
        public App ModifyIconUrl(String iconUrl)
        {
            if (String.IsNullOrEmpty(iconUrl))
            {
                throw new ArgumentException($@"{nameof(iconUrl)} 不能为空");
            }

            if (iconUrl == IconUrl)
            {
                return this;
            }

            IconUrl = iconUrl;
            OnChanged(nameof(IconUrl));
            return this;
        }

        /// <summary>
        /// 修改app宽
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public App ModifyWidth(Int32 width)
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

        /// <summary>
        /// 修改app高
        /// </summary>
        public App ModifyHeight(Int32 height)
        {
            if (height <= 0)
            {
                throw new ArgumentException($@"{nameof(height)} 不能小于或等于0");
            }

            if (height == Height)
            {
                return this;
            }

            OnChanged(nameof(Height));
            return this;
        }

        /// <summary>
        /// 使用人数+1
        /// </summary>
        public App IncreaseUseCount()
        {
            UseCount += 1;
            OnChanged(nameof(UseCount));
            return this;
        }

        /// <summary>
        /// 使用人数-1
        /// </summary>
        /// <returns></returns>
        public App DecreaseUseCount()
        {
            UseCount -= 1;
            OnChanged(nameof(UseCount));
            return this;
        }

        /// <summary>
        /// 展示到任务栏
        /// </summary>
        public App Setbar()
        {
            IsSetbar = true;
            OnChanged(nameof(IsSetbar));
            return this;
        }

        /// <summary>
        /// 不在任务栏展示
        /// </summary>
        public App NotSetbar()
        {
            IsSetbar = false;
            OnChanged(nameof(IsSetbar));
            return this;
        }

        /// <summary>
        /// 打开时最大化
        /// </summary>
        public App OpenMax()
        {
            IsOpenMax = true;
            OnChanged(nameof(IsOpenMax));
            return this;
        }

        /// <summary>
        /// 打开时不进行最大化
        /// </summary>
        public App NotOpenMax()
        {
            IsOpenMax = false;
            OnChanged(nameof(IsOpenMax));
            return this;
        }

        /// <summary>
        /// 是系统app
        /// </summary>
        public App System()
        {
            IsSystem = true;
            OnChanged(nameof(IsSystem));
            return this;
        }

        /// <summary>
        /// 不是系统app
        /// </summary>
        public App NotSystem()
        {
            IsSystem = false;
            OnChanged(nameof(IsSystem));
            return this;
        }

        /// <summary>
        /// 是福莱希
        /// </summary>
        public App Flash()
        {
            IsFlash = true;
            OnChanged(nameof(IsFlash));
            return this;
        }

        /// <summary>
        /// 不是福莱希
        /// </summary>
        public App NotFlash()
        {
            IsFlash = false;
            OnChanged(nameof(IsFlash));
            return this;
        }

        /// <summary>
        /// 允许改变app大小
        /// </summary>
        public App Resize()
        {
            IsResize = true;
            OnChanged(nameof(IsResize));
            return this;
        }

        /// <summary>
        /// 允许改变app大小
        /// </summary>
        public App NotResize()
        {
            IsResize = false;
            OnChanged(nameof(IsResize));
            return this;
        }

        /// <summary>
        /// 修改应用类型
        /// </summary>
        public App ModifyAppTypeId(Int32 appTypeId)
        {
            if (appTypeId == AppTypeId)
            {
                return this;
            }

            AppTypeId = appTypeId;
            OnChanged(nameof(appTypeId));
            return this;
        }

        /// <summary>
        /// 推荐
        /// </summary>
        public App Recommand()
        {
            IsRecommand = true;
            OnChanged(nameof(IsRecommand));
            return this;
        }

        /// <summary>
        /// 取消推荐
        /// </summary>
        public App CancelRecommand()
        {
            IsRecommand = false;
            OnChanged(nameof(IsRecommand));
            return this;
        }

        public App AppRelease()
        {
            AppReleaseState = AppReleaseState.Release;
            OnChanged(nameof(AppReleaseState));
            return this;
        }

        public App AppUnrelease()
        {
            AppReleaseState = AppReleaseState.UnRelease;
            OnChanged(nameof(AppReleaseState));
            return this;
        }

        public App Wait()
        {
            AppAuditState = AppAuditState.Wait;
            OnChanged(nameof(AppAuditState));
            return this;
        }

        public App Pass()
        {
            AppAuditState = AppAuditState.Pass;
            OnChanged(nameof(AppAuditState));
            return this;
        }

        public App Deny()
        {
            AppAuditState = AppAuditState.Deny;
            OnChanged(nameof(AppAuditState));
            return this;
        }

        public App UnAuditState()
        {
            AppAuditState = AppAuditState.UnAuditState;
            OnChanged(nameof(AppAuditState));
            return this;
        }

        public App StyleToApp()
        {
            AppStyle = AppStyle.App;
            OnChanged(nameof(AppStyle));
            return this;
        }

        public App StyleToWidget()
        {
            AppStyle = AppStyle.Widget;
            OnChanged(nameof(AppStyle));
            return this;
        }

        public App IconNotFromUpload()
        {
            IsIconByUpload = false;
            OnChanged(nameof(IsIconByUpload));
            return this;
        }

        public App IconFromUpload()
        {
            IsIconByUpload = true;
            OnChanged(nameof(IsIconByUpload));
            return this;
        }

        public App ModifyUrl(String newAppUrl)
        {
            if (String.IsNullOrEmpty(newAppUrl))
            {
                throw new ArgumentException($@"{nameof(newAppUrl)} 不能为空");
            }

            if (newAppUrl == AppUrl)
            {
                return this;
            }

            AppUrl = newAppUrl;
            OnChanged(nameof(AppUrl));
            return this;
        }

        public App ModifyRemark(String newRemark)
        {
            if (newRemark == Remark)
            {
                return this;
            }

            Remark = newRemark ?? "";
            OnChanged(nameof(Remark));
            return this;
        }
    }
}
