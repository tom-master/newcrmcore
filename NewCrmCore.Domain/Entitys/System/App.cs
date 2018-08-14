using System;
using System.ComponentModel;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{

    [Serializable, Description("应用")]
    public partial class App : DomainModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [PropertyRequired, InputRange(2, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        [PropertyRequired, InputRange(150)]
        public String IconUrl { get; private set; }

        /// <summary>
        /// app地址
        /// </summary>
        [PropertyRequired, InputRange(150)]
        public String AppUrl { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [InputRange(50), PropertyDefaultValue(typeof(String), "")]
        public String Remark { get; private set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [PropertyRequired]
        public Int32 Width { get; private set; }

        /// <summary>
        /// 高度
        /// </summary>
        [PropertyRequired]
        public Int32 Height { get; private set; }

        /// <summary>
        /// 使用数
        /// </summary>
        [PropertyDefaultValue(typeof(Int32), 0)]
        public Int32 UseCount { get; private set; }

        /// <summary>
        /// 是否显示app底部的按钮
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsSetbar { get; private set; }

        /// <summary>
        /// 是否打开最大化
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsOpenMax { get; private set; }

        /// <summary>
        /// 是否为系统应用
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsSystem { get; private set; }

        /// <summary>
        /// 是否为福莱希
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsFlash { get; private set; }

        /// <summary>
        /// 是否可以拉伸
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsResize { get; private set; }

        /// <summary>
        /// 开发者(用户)Id
        /// </summary>
        [PropertyRequired]
        public Int32 AccountId { get; private set; }

        /// <summary>
        /// App类型Id
        /// </summary>
        [PropertyRequired]
        public Int32 AppTypeId { get; private set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsRecommand { get; private set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [PropertyDefaultValue(typeof(AppAuditState), AppAuditState.UnAuditState)]
        public AppAuditState AppAuditState { get; private set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        [PropertyDefaultValue(typeof(AppReleaseState), AppReleaseState.UnRelease)]
        public AppReleaseState AppReleaseState { get; private set; }

        /// <summary>
        /// app样式
        /// </summary>
        [PropertyRequired]
        public AppStyle AppStyle { get; private set; }

        /// <summary>
        /// 是否安装
        /// </summary>
        public Boolean IsInstall { get; private set; }

        public Double StarCount { get; private set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public String AccountName { get; private set; }

        /// <summary>
        /// 图标是否来自上传
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
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
            Int32 accountId = default(Int32),
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
            if (accountId == 0)
            {
                IsSystem = true;
            }
            else
            {
                IsSystem = false;
                AccountId = accountId;
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
                throw new ArgumentException($@"{nameof(appName)} is null");
            }

            Name = appName;
            OnPropertyChanged(new PropertyArgs(nameof(Name), Name));
            return this;
        }

        /// <summary>
        /// 修改app图标
        /// </summary>
        public App ModifyIconUrl(String iconUrl)
        {
            if (String.IsNullOrEmpty(iconUrl))
            {
                throw new ArgumentException($@"{nameof(iconUrl)} is null");
            }

            IconUrl = iconUrl;
            OnPropertyChanged(new PropertyArgs(nameof(IconUrl), IconUrl));
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
                throw new ArgumentException($@"{nameof(width)} less than or equal to zero");
            }

            Width = width;
            OnPropertyChanged(new PropertyArgs(nameof(Width), Width));
            return this;
        }

        /// <summary>
        /// 修改app高
        /// </summary>
        public App ModifyHeight(Int32 height)
        {
            if (height <= 0)
            {
                throw new ArgumentException($@"{nameof(height)} less than or equal to zero");
            }

            Height = height;

            OnPropertyChanged(new PropertyArgs(nameof(Height), Height));
            return this;
        }

        /// <summary>
        /// 使用人数+1
        /// </summary>
        public App IncreaseUseCount()
        {
            UseCount += 1;
            OnPropertyChanged(new PropertyArgs(nameof(UseCount), UseCount));
            return this;
        }

        /// <summary>
        /// 使用人数-1
        /// </summary>
        /// <returns></returns>
        public App DecreaseUseCount()
        {
            UseCount -= 1;
            OnPropertyChanged(new PropertyArgs(nameof(UseCount), UseCount));
            return this;
        }

        /// <summary>
        /// 展示到任务栏
        /// </summary>
        public App Setbar()
        {
            IsSetbar = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsSetbar), IsSetbar));
            return this;
        }

        /// <summary>
        /// 不在任务栏展示
        /// </summary>
        public App NotSetbar()
        {
            IsSetbar = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsSetbar), IsSetbar));
            return this;
        }

        /// <summary>
        /// 打开时最大化
        /// </summary>
        public App OpenMax()
        {
            IsOpenMax = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsOpenMax), IsOpenMax));
            return this;
        }

        /// <summary>
        /// 打开时不进行最大化
        /// </summary>
        public App NotOpenMax()
        {
            IsOpenMax = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsOpenMax), IsOpenMax));
            return this;
        }

        /// <summary>
        /// 是系统app
        /// </summary>
        public App System()
        {
            IsSystem = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsSystem), IsSystem));
            return this;
        }

        /// <summary>
        /// 不是系统app
        /// </summary>
        public App NotSystem()
        {
            IsSystem = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsSystem), IsSystem));
            return this;
        }

        /// <summary>
        /// 是福莱希
        /// </summary>
        public App Flash()
        {
            IsFlash = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsFlash), IsFlash));
            return this;
        }

        /// <summary>
        /// 不是福莱希
        /// </summary>
        public App NotFlash()
        {
            IsFlash = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsFlash), IsFlash));
            return this;
        }

        /// <summary>
        /// 允许改变app大小
        /// </summary>
        public App Resize()
        {
            IsResize = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsResize), IsResize));
            return this;
        }

        /// <summary>
        /// 允许改变app大小
        /// </summary>
        public App NotResize()
        {
            IsResize = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsResize), IsResize));
            return this;
        }

        /// <summary>
        /// 修改应用类型
        /// </summary>
        public App ModifyAppTypeId(Int32 appTypeId)
        {
            AppTypeId = appTypeId;
            OnPropertyChanged(new PropertyArgs(nameof(AppTypeId), AppTypeId));
            return this;
        }

        /// <summary>
        /// 推荐
        /// </summary>
        public App Recommand()
        {
            IsRecommand = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsRecommand), IsRecommand));
            return this;
        }

        /// <summary>
        /// 取消推荐
        /// </summary>
        public App CancelRecommand()
        {
            IsRecommand = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsRecommand), IsRecommand));
            return this;
        }

        public App AppRelease()
        {
            AppReleaseState = AppReleaseState.Release;
            OnPropertyChanged(new PropertyArgs(nameof(AppReleaseState), AppReleaseState));
            return this;
        }

        public App AppUnrelease()
        {
            AppReleaseState = AppReleaseState.UnRelease;
            OnPropertyChanged(new PropertyArgs(nameof(AppReleaseState), AppReleaseState));
            return this;
        }

        public App Wait()
        {
            AppAuditState = AppAuditState.Wait;
            OnPropertyChanged(new PropertyArgs(nameof(AppAuditState), AppAuditState));
            return this;
        }

        public App Pass()
        {
            AppAuditState = AppAuditState.Pass;
            OnPropertyChanged(new PropertyArgs(nameof(AppAuditState), AppAuditState));
            return this;
        }

        public App Deny()
        {
            AppAuditState = AppAuditState.Deny;
            OnPropertyChanged(new PropertyArgs(nameof(AppAuditState), AppAuditState));
            return this;
        }

        public App UnAuditState()
        {
            AppAuditState = AppAuditState.UnAuditState;
            OnPropertyChanged(new PropertyArgs(nameof(AppAuditState), AppAuditState));
            return this;
        }

        public App StyleToApp()
        {
            AppStyle = AppStyle.App;
            OnPropertyChanged(new PropertyArgs(nameof(AppStyle), AppStyle));
            return this;
        }

        public App StyleToWidget()
        {
            AppStyle = AppStyle.Widget;
            OnPropertyChanged(new PropertyArgs(nameof(AppStyle), AppStyle));
            return this;
        }

        public App IconNotFromUpload()
        {
            IsIconByUpload = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsIconByUpload), IsIconByUpload));
            return this;
        }

        public App IconFromUpload()
        {
            IsIconByUpload = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsIconByUpload), IsIconByUpload));
            return this;
        }

        public App ModifyUrl(String newAppUrl)
        {
            if (String.IsNullOrEmpty(newAppUrl))
            {
                throw new ArgumentException($@"{nameof(newAppUrl)} is null");
            }

            AppUrl = newAppUrl;
            OnPropertyChanged(new PropertyArgs(nameof(AppUrl), AppUrl));
            return this;
        }

        public App ModifyRemark(String newRemark)
        {
            Remark = newRemark ?? "";
            OnPropertyChanged(new PropertyArgs(nameof(Remark), Remark));
            return this;
        }
    }
}
