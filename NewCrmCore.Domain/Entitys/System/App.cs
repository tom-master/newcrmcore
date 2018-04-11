using System;
using System.ComponentModel;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{

	[Serializable, Description("应用")]
	public partial class App: DomainModelBase
	{
		#region private field

		public String _name;

		public String _iconUrl;

		public String _appUrl;

		public String _remark;

		public Int32 _width;

		public Int32 _height;

		public Int32 _useCount;

		public Boolean _isMax;

		public Boolean _isFull;

		public Boolean _isSetbar;

		public Boolean _isOpenMax;

		public Boolean _isLock;

		public Boolean _isSystem;

		public Boolean _isFlash;

		public Boolean _isDraw;

		public Boolean _isResize;

		public Int32 _accountId;

		public Int32 _appTypeId;

		public Boolean _isRecommand;

		public AppAuditState _appAuditState;

		public AppReleaseState _appReleaseState;

		public AppStyle _appStyle;

		public Boolean _isInstall;

		public Double _starCount;

		public String _accountName;

		public Boolean _isIconByUpload;
		#endregion

		#region public property

		/// <summary>
		/// 名称
		/// </summary>
		[PropertyRequired, InputRange(6)]
		public String Name
		{
			get { return _name; }
			private set
			{
				_name = value;
			}
		}

		/// <summary>
		/// 图标地址
		/// </summary>
		[PropertyRequired]
		public String IconUrl
		{
			get { return _iconUrl; }
			private set
			{
				_iconUrl = value;
			}
		}

		/// <summary>
		/// app地址
		/// </summary>
		[PropertyRequired]
		public String AppUrl
		{
			get { return _appUrl; }
			private set
			{
				_appUrl = value;
			}
		}

		/// <summary>
		/// 备注
		/// </summary>
		[InputRange(50), PropertyDefaultValue(typeof(String), "")]
		public String Remark
		{
			get { return _remark; }
			private set
			{
				_remark = value;
			}
		}

		/// <summary>
		/// 宽度
		/// </summary>
		[PropertyRequired]
		public Int32 Width
		{
			get { return _width; }
			private set
			{
				_width = value;
			}
		}

		/// <summary>
		/// 高度
		/// </summary>
		[PropertyRequired]
		public Int32 Height
		{
			get { return _height; }
			private set
			{
				_height = value;
			}
		}

		/// <summary>
		/// 使用数
		/// </summary>
		[PropertyDefaultValue(typeof(Int32), 0)]
		public Int32 UseCount
		{
			get { return _useCount; }
			private set
			{
				_useCount = value;
			}
		}

		/// <summary>
		/// 是否能最大化
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsMax
		{
			get { return _isMax; }
			private set
			{
				_isMax = value;
			}
		}

		/// <summary>
		/// 是否打开后铺满全屏
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsFull
		{
			get { return _isFull; }
			private set
			{
				_isFull = value;
			}
		}

		/// <summary>
		/// 是否显示app底部的按钮
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsSetbar
		{
			get { return _isSetbar; }
			private set
			{
				_isSetbar = value;
			}
		}

		/// <summary>
		/// 是否打开最大化
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsOpenMax
		{
			get { return _isOpenMax; }
			private set
			{
				_isOpenMax = value;
			}
		}

		/// <summary>
		/// 是否锁定
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsLock
		{
			get { return _isLock; }
			private set
			{
				_isLock = value;
			}
		}

		/// <summary>
		/// 是否为系统应用
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsSystem
		{
			get { return _isSystem; }
			private set
			{
				_isSystem = value;
			}
		}

		/// <summary>
		/// 是否为福莱希
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsFlash
		{
			get { return _isFlash; }
			private set
			{
				_isFlash = value;
			}
		}

		/// <summary>
		/// 是否可以拖动
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsDraw
		{
			get { return _isDraw; }
			private set
			{
				_isDraw = value;
			}
		}

		/// <summary>
		/// 是否可以拉伸
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsResize
		{
			get { return _isResize; }
			private set
			{
				_isResize = value;
			}
		}

		/// <summary>
		/// 开发者(用户)Id
		/// </summary>
		[PropertyRequired]
		public Int32 AccountId
		{
			get { return _accountId; }
			private set
			{
				_accountId = value;
			}
		}

		/// <summary>
		/// App类型Id
		/// </summary>
		[PropertyRequired]
		public Int32 AppTypeId
		{
			get { return _appTypeId; }
			private set
			{
				_appTypeId = value;
			}
		}

		/// <summary>
		/// 是否推荐
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsRecommand
		{
			get { return _isRecommand; }
			private set
			{
				_isRecommand = value;
			}
		}

		/// <summary>
		/// 审核状态
		/// </summary>
		[PropertyDefaultValue(typeof(AppAuditState), AppAuditState.UnAuditState)]
		public AppAuditState AppAuditState
		{
			get { return _appAuditState; }
			private set
			{
				_appAuditState = value;
			}
		}

		/// <summary>
		/// 发布状态
		/// </summary>
		[PropertyDefaultValue(typeof(AppReleaseState), AppReleaseState.UnRelease)]
		public AppReleaseState AppReleaseState
		{
			get { return _appReleaseState; }
			private set
			{
				_appReleaseState = value;
			}
		}

		/// <summary>
		/// app样式
		/// </summary>
		[PropertyRequired]
		public AppStyle AppStyle
		{
			get { return _appStyle; }
			private set
			{
				_appStyle = value;
			}
		}

		/// <summary>
		/// 是否安装
		/// </summary>
		public Boolean IsInstall
		{
			get { return _isInstall; }
			private set
			{
				_isInstall = value;
			}
		}

		public Double StarCount
		{
			get { return _starCount; }
			private set
			{
				_starCount = value;
			}
		}

		public String AccountName
		{
			get { return _accountName; }
			private set
			{
				_accountName = value;
			}
		}

		/// <summary>
		/// 图标是否来自上传
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsIconByUpload
		{
			get { return _isIconByUpload; }
			private set
			{
				_isIconByUpload = value;
			}
		}

		#endregion

		#region ctor

		/// <summary>
		/// 实例化一个app对象
		/// </summary>
		public App(String name,
			String iconUrl,
			String appUrl,
			Int32 width,
			Int32 height,
			Int32 appTypeId,
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

			IsLock = false;
			Remark = remark;
			AppAuditState = appAuditState;
			AppReleaseState = appReleaseState;
			UseCount = 0;
			IsRecommand = false;
			IsIconByUpload = isIconByUpload;
		}

		public App() { }
		#endregion
	}

	/// <summary>
	/// AppExtension
	/// </summary>
	public partial class App
	{

		public App ModifyName(String appName)
		{
			if (String.IsNullOrEmpty(appName))
			{
				throw new ArgumentException($@"{nameof(appName)} is null");
			}

			Name = appName;
			OnPropertyChanged(nameof(appName));
			return this;
		}

		public App ModifyIconUrl(String iconUrl)
		{
			if (String.IsNullOrEmpty(iconUrl))
			{
				throw new ArgumentException($@"{nameof(iconUrl)} is null");
			}

			IconUrl = iconUrl;
			OnPropertyChanged(nameof(iconUrl));
			return this;
		}

		public App ModifyWidth(Int32 width)
		{
			if (width <= 0)
			{
				throw new ArgumentException($@"{nameof(width)} less than or equal to zero");
			}

			Width = width;
			OnPropertyChanged(nameof(Width));
			return this;
		}

		public App ModifyHeight(Int32 height)
		{
			if (height <= 0)
			{
				throw new ArgumentException($@"{nameof(height)} less than or equal to zero");
			}

			Height = height;
			OnPropertyChanged(nameof(Height));
			return this;
		}

		public App IncreaseUseCount()
		{
			UseCount += 1;
			OnPropertyChanged(nameof(UseCount));
			return this;
		}

		public App DecreaseUseCount()
		{
			UseCount -= 1;
			OnPropertyChanged(nameof(UseCount));
			return this;
		}

		public App Max()
		{
			IsMax = true;
			OnPropertyChanged(nameof(IsMax));
			return this;
		}

		public App NotMax()
		{
			IsMax = false;
			OnPropertyChanged(nameof(IsMax));
			return this;
		}

		public App Full()
		{
			IsFull = true;
			OnPropertyChanged(nameof(IsFull));
			return this;
		}

		public App NotFull()
		{
			IsFull = false;
			OnPropertyChanged(nameof(IsFull));
			return this;
		}

		public App Setbar()
		{
			IsSetbar = true;
			OnPropertyChanged(nameof(IsSetbar));
			return this;
		}

		public App NotSetbar()
		{
			IsSetbar = false;
			OnPropertyChanged(nameof(IsSetbar));
			return this;
		}

		public App OpenMax()
		{
			IsOpenMax = true;
			OnPropertyChanged(nameof(IsOpenMax));
			return this;
		}

		public App NotOpenMax()
		{
			IsOpenMax = false;
			OnPropertyChanged(nameof(IsOpenMax));
			return this;
		}

		public App Lock()
		{
			IsLock = true;
			OnPropertyChanged(nameof(IsLock));
			return this;
		}

		public App NotLock()
		{
			IsLock = false;
			OnPropertyChanged(nameof(IsLock));
			return this;
		}


		public App System()
		{
			IsSystem = true;
			OnPropertyChanged(nameof(IsSystem));
			return this;
		}

		public App NotSystem()
		{
			IsSystem = false;
			OnPropertyChanged(nameof(IsSystem));
			return this;
		}

		public App Flash()
		{
			IsFlash = true;
			OnPropertyChanged(nameof(IsFlash));
			return this;
		}

		public App NotFlash()
		{
			IsFlash = false;
			OnPropertyChanged(nameof(IsFlash));
			return this;
		}

		public App Draw()
		{
			IsDraw = true;
			OnPropertyChanged(nameof(IsDraw));
			return this;
		}

		public App NotDraw()
		{
			IsDraw = false;
			OnPropertyChanged(nameof(IsDraw));
			return this;
		}

		public App Resize()
		{
			IsResize = true;
			OnPropertyChanged(nameof(IsResize));
			return this;
		}

		public App NotResize()
		{
			IsResize = false;
			OnPropertyChanged(nameof(IsResize));
			return this;
		}

		public App ModifyAppTypeId(Int32 appTypeId)
		{
			AppTypeId = appTypeId;
			OnPropertyChanged(nameof(AppTypeId));
			return this;
		}

		public App Recommand()
		{
			IsRecommand = true;
			OnPropertyChanged(nameof(IsRecommand));
			return this;
		}

		public App CancelRecommand()
		{
			IsRecommand = false;
			OnPropertyChanged(nameof(IsRecommand));
			return this;
		}

		public App AppRelease()
		{
			AppReleaseState = AppReleaseState.Release;
			OnPropertyChanged(nameof(AppReleaseState));
			return this;
		}

		public App AppUnrelease()
		{
			AppReleaseState = AppReleaseState.UnRelease;
			OnPropertyChanged(nameof(AppReleaseState));
			return this;
		}

		public App Wait()
		{
			AppAuditState = AppAuditState.Wait;
			OnPropertyChanged(nameof(AppAuditState));
			return this;
		}

		public App Pass()
		{
			AppAuditState = AppAuditState.Pass;
			OnPropertyChanged(nameof(AppAuditState));
			return this;
		}

		public App Deny()
		{
			AppAuditState = AppAuditState.Deny;
			OnPropertyChanged(nameof(AppAuditState));
			return this;
		}

		public App UnAuditState()
		{
			AppAuditState = AppAuditState.UnAuditState;
			OnPropertyChanged(nameof(AppAuditState));
			return this;
		}

		public App StyleToApp()
		{
			AppStyle = AppStyle.App;
			OnPropertyChanged(nameof(AppStyle));
			return this;
		}

		public App StyleToWidget()
		{
			AppStyle = AppStyle.Widget;
			OnPropertyChanged(nameof(AppStyle));
			return this;
		}

		public App IconNotFromUpload()
		{
			IsIconByUpload = false;
			OnPropertyChanged(nameof(IsIconByUpload));
			return this;
		}

		public App IconFromUpload()
		{
			IsIconByUpload = true;
			OnPropertyChanged(nameof(IsIconByUpload));
			return this;
		}

		public App ModifyUrl(String newAppUrl)
		{
			if (String.IsNullOrEmpty(newAppUrl))
			{
				throw new ArgumentException($@"{nameof(newAppUrl)} is null");
			}

			AppUrl = newAppUrl;
			OnPropertyChanged(nameof(AppUrl));
			return this;
		}

		public App ModifyRemark(String newRemark)
		{
			if (String.IsNullOrEmpty(newRemark))
			{
				throw new ArgumentException($@"{nameof(newRemark)} is null");
			}
			Remark = newRemark;
			OnPropertyChanged(nameof(Remark));
			return this;
		}
	}
}
