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
	public partial class Member: DomainModelBase
	{
		#region public property

		/// <summary>
		/// 应用Id
		/// </summary>
		[PropertyRequired]
		public Int32 AppId
		{
			get; private set;
		}

		/// <summary>
		/// 成员的宽
		/// </summary>
		[PropertyRequired]
		public Int32 Width
		{
			get; private set;
		}

		/// <summary>
		/// 成员的高
		/// </summary>
		[PropertyRequired]
		public Int32 Height
		{
			get; private set;
		}

		/// <summary>
		/// 文件夹Id
		/// </summary>
		[PropertyDefaultValue(typeof(Int32), 0)]
		public Int32 FolderId
		{
			get; private set;
		}

		/// <summary>
		/// 名称
		/// </summary>
		[PropertyRequired, InputRange(10)]
		public String Name
		{
			get; private set;
		}

		/// <summary>
		/// 图标地址
		/// </summary>
		[PropertyRequired, InputRange(150)]
		public String IconUrl
		{
			get; private set;
		}

		/// <summary>
		/// app地址
		/// </summary>
		[PropertyDefaultValue(typeof(String), ""), InputRange(150)]
		public String AppUrl
		{
			get; private set;
		}

		/// <summary>
		/// 成员是否在应用码头上
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsOnDock
		{
			get; private set;
		}

		/// <summary>
		/// 是否能最大化
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsMax
		{
			get; private set;
		}

		/// <summary>
		/// 是否打开后铺满全屏
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsFull
		{
			get; private set;
		}

		/// <summary>
		/// 是否显示app底部的按钮
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsSetbar
		{
			get; private set;
		}

		/// <summary>
		/// 是否打开最大化
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsOpenMax
		{
			get; private set;
		}

		/// <summary>
		/// 是否锁定
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsLock
		{
			get; private set;
		}

		/// <summary>
		/// 是否为福莱希
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsFlash
		{
			get; private set;
		}

		/// <summary>
		/// 是否可以拖动
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsDraw
		{
			get; private set;
		}

		/// <summary>
		/// 是否可以拉伸
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsResize
		{
			get; private set;
		}

		/// <summary>
		/// 成员类型
		/// </summary>
		[PropertyRequired]
		public MemberType MemberType
		{
			get; private set;
		}

		/// <summary>
		/// 桌面索引
		/// </summary>
		[PropertyDefaultValue(typeof(Int32), 1)]
		public Int32 DeskIndex
		{
			get; private set;
		}

		/// <summary>
		/// 账户Id
		/// </summary>
		[PropertyRequired]
		public Int32 AccountId
		{
			get; private set;
		}

		/// <summary>
		/// 图标是否来自上传
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsIconByUpload
		{
			get; private set;
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
