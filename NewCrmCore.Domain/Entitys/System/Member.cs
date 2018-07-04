using System;
using System.ComponentModel;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
	/// <summary>
	/// 成员
	/// </summary>
	[Serializable, Description("成员")]
	public partial class Member : DomainModelBase
	{
		#region private field

		private Int32 _appId;

		private Int32 _width;

		private Int32 _height;

		private Int32 _folderId;

		private String _name;

		private String _iconUrl;

		private String _appUrl;

		private Boolean _isOnDock;

		private Boolean _isMax;

		private Boolean _isFull;

		private Boolean _isSetbar;

		private Boolean _isOpenMax;

		private Boolean _isLock;

		private Boolean _isFlash;

		private Boolean _isDraw;

		private Boolean _isResize;

		private MemberType _memberType;

		private Int32 _deskIndex;

		private Int32 _accountId;

		private Boolean _isIconByUpload;

		#endregion

		#region public property

		/// <summary>
		/// 应用Id
		/// </summary>
		[PropertyRequired]
		public Int32 AppId
		{
			get { return _appId; }
			private set
			{
				_appId = value;
			}
		}

		/// <summary>
		/// 成员的宽
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
		/// 成员的高
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
		/// 文件夹Id
		/// </summary>
		[PropertyDefaultValue(typeof(Int32), 0)]
		public Int32 FolderId
		{
			get { return _folderId; }
			private set
			{
				_folderId = value;
			}
		}

		/// <summary>
		/// 名称
		/// </summary>
		[PropertyRequired, InputRange(10)]
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
		[PropertyRequired, InputRange(150)]
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
		[PropertyRequired, InputRange(150)]
		public String AppUrl
		{
			get { return _appUrl; }
			private set
			{
				_appUrl = value;
			}
		}

		/// <summary>
		/// 成员是否在应用码头上
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsOnDock
		{
			get { return _isOnDock; }
			private set
			{
				_isOnDock = value;
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
		/// 成员类型
		/// </summary>
		[PropertyRequired]
		public MemberType MemberType
		{
			get { return _memberType; }
			private set
			{
				_memberType = value;
			}
		}

		/// <summary>
		/// 桌面索引
		/// </summary>
		[PropertyDefaultValue(typeof(Int32), 1)]
		public Int32 DeskIndex
		{
			get { return _deskIndex; }
			private set
			{
				_deskIndex = value;
			}
		}

		/// <summary>
		/// 账户Id
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

		#region public ctor

		/// <summary>
		/// 实例化一个成员对象
		/// </summary>
		public Member(
			String name,
			String iconUrl,
			String appUrl,
			Int32 appId,
			Int32 width,
			Int32 height,
			Int32 accountId,
			Int32 deskIndex,
			Boolean isIconByUpload = default(Boolean),
			Boolean isLock = default(Boolean),
			Boolean isMax = default(Boolean),
			Boolean isFull = default(Boolean),
			Boolean isSetbar = default(Boolean),
			Boolean isOpenMax = default(Boolean),
			Boolean isFlash = default(Boolean),
			Boolean isDraw = default(Boolean),
			Boolean isResize = default(Boolean))
		{
			AppId = appId;
			Width = width > 800 ? 800 : width;
			Height = height > 600 ? 600 : height;
			IsDraw = isDraw;
			IsOpenMax = isOpenMax;
			IsSetbar = isSetbar;
			IsMax = isMax;
			IsFull = isFull;
			IsFlash = isFlash;
			IsResize = isResize;
			IsLock = isLock;
			Name = name;
			IconUrl = iconUrl;
			AppUrl = appUrl;
			MemberType = appId == 0 ? MemberType.Folder : MemberType.App;
			DeskIndex = 1;
			IsIconByUpload = isIconByUpload;
			AccountId = accountId;
		}

		/// <summary>
		/// 实例化一个成员对象
		/// </summary>
		public Member(String name,
			String iconUrl,
			Int32 appId,
			Int32 accountId,
			Int32 deskIndex,
			Boolean isIconByUpload = default(Boolean))
		{
			AppId = appId;
			Width = 800;
			Height = 600;
			IsDraw = false;
			IsOpenMax = false;
			Name = name;
			IconUrl = iconUrl;
			DeskIndex = deskIndex;
			MemberType = appId == 0 ? MemberType.Folder : MemberType.App;
			IsIconByUpload = isIconByUpload;
			AccountId = accountId;
		}

		public Member()
		{
		}

		#endregion
	}

	/// <summary>
	/// MemberExtension
	/// </summary>
	public partial class Member
	{

		public Member ModifyWidth(Int32 width)
		{
			if (width <= 0)
			{
				throw new ArgumentException($@"{nameof(width)} less than or equal to zero");
			}

			Width = width;
			OnPropertyChanged(new PropertyArgs(nameof(Width), Width));
			return this;
		}

		public Member ModifyHeight(Int32 height)
		{
			if (height <= 0)
			{
				throw new ArgumentException($@"{nameof(height)} less than or equal to zero");
			}

			Height = height;
			OnPropertyChanged(new PropertyArgs(nameof(Height), Height));
			return this;
		}

		public Member ModifyFolderId(Int32 folderId)
		{
			FolderId = folderId;
			OnPropertyChanged(new PropertyArgs(nameof(FolderId), FolderId));
			return this;
		}

		public Member ModifyName(String name)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException($@"{nameof(name)} is null");
			}

			Name = name;
			OnPropertyChanged(new PropertyArgs(nameof(Name), Name));
			return this;
		}

		public Member ModifyIconUrl(String iconUrl)
		{
			if (String.IsNullOrEmpty(iconUrl))
			{
				throw new ArgumentException($@"{nameof(iconUrl)} is null");
			}

			IconUrl = iconUrl;
			OnPropertyChanged(new PropertyArgs(nameof(IconUrl), IconUrl));
			return this;
		}

		public Member ModifyAppUrl(String appUrl)
		{
			if (String.IsNullOrEmpty(appUrl))
			{
				throw new ArgumentException($@"{nameof(appUrl)} is null");
			}

			AppUrl = appUrl;
			OnPropertyChanged(new PropertyArgs(nameof(AppUrl), AppUrl));
			return this;
		}

		public Member OnDock()
		{
			IsOnDock = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsOnDock), IsOnDock));
			return this;
		}

		public Member OutDock()
		{
			IsOnDock = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsOnDock), IsOnDock));
			return this;
		}

		public Member Max()
		{
			IsMax = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsMax), IsMax));
			return this;
		}

		public Member NotMax()
		{
			IsMax = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsMax), IsMax));
			return this;
		}

		public Member Full()
		{
			IsFull = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsFull), IsFull));
			return this;
		}

		public Member NotFull()
		{
			IsFull = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsFull), IsFull));
			return this;
		}

		public Member Setbar()
		{
			IsSetbar = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsSetbar), IsSetbar));
			return this;
		}

		public Member NotSetbar()
		{
			IsSetbar = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsSetbar), IsSetbar));
			return this;
		}

		public Member OpenMax()
		{
			IsOpenMax = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsOpenMax), IsOpenMax));
			return this;
		}

		public Member NotOpenMax()
		{
			IsOpenMax = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsOpenMax), IsOpenMax));
			return this;
		}

		public Member Lock()
		{
			IsLock = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsLock), IsLock));
			return this;
		}

		public Member NotLock()
		{
			IsLock = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsLock), IsLock));
			return this;
		}

		public Member Flash()
		{
			IsFlash = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsFlash), IsFlash));
			return this;
		}

		public Member NotFlash()
		{
			IsFlash = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsFlash), IsFlash));
			return this;
		}

		public Member Draw()
		{
			IsDraw = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsDraw), IsDraw));
			return this;
		}

		public Member NotDraw()
		{
			IsDraw = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsDraw), IsDraw));
			return this;
		}

		public Member Resize()
		{
			IsResize = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsResize), IsResize));
			return this;
		}

		public Member NotResize()
		{
			IsResize = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsResize), IsResize));
			return this;
		}

		public Member ModifyDeskIndex(Int32 deskIndex)
		{
			if (deskIndex <= 0)
			{
				throw new ArgumentException($@"{nameof(deskIndex)} less than or equal to zero");
			}

			DeskIndex = deskIndex;
			OnPropertyChanged(new PropertyArgs(nameof(DeskIndex), DeskIndex));
			return this;
		}

		public Member IconNotFromUpload()
		{
			IsIconByUpload = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsIconByUpload), IsIconByUpload));
			return this;
		}

		public Member IconFromUpload()
		{
			IsIconByUpload = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsIconByUpload), IsIconByUpload));
			return this;
		}
	}
}
