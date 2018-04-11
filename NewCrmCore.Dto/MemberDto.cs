using System;
using System.ComponentModel.DataAnnotations;

namespace NewCrmCore.Dto
{
	public sealed class MemberDto: BaseDto
	{

		/// <summary>
		/// 应用Id
		/// </summary>
		public Int32 AppId { get; set; }

		/// <summary>
		/// 成员的宽
		/// </summary>
		public Int32 Width { get; set; }

		/// <summary>
		/// 成员的高
		/// </summary>
		public Int32 Height { get; set; }

		/// <summary>
		/// 文件夹Id
		/// </summary>
		public Int32 FolderId { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[Required]
		public String Name { get; set; }

		/// <summary>
		/// 图标地址
		/// </summary>
		[Required]
		public String IconUrl { get; set; }

		/// <summary>
		/// app地址
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		public String AppUrl { get; set; }

		/// <summary>
		/// 成员是否在应用码头上
		/// </summary>

		public Boolean IsOnDock { get; set; }

		/// <summary>
		/// 是否能最大化
		/// </summary>
		public Boolean IsMax { get; set; }

		/// <summary>
		/// 是否打开后铺满全屏
		/// </summary>
		public Boolean IsFull { get; set; }

		/// <summary>
		/// 是否显示app底部的按钮
		/// </summary>
		public Boolean IsSetbar { get; set; }

		/// <summary>
		/// 是否打开最大化
		/// </summary>
		public Boolean IsOpenMax { get; set; }

		/// <summary>
		/// 是否锁定
		/// </summary>
		public Boolean IsLock { get; set; }

		/// <summary>
		/// 是否为福莱希
		/// </summary>
		public Boolean IsFlash { get; set; }

		/// <summary>
		/// 是否可以拖动
		/// </summary>
		public Boolean IsDraw { get; set; }

		/// <summary>
		/// 是否可以拉伸
		/// </summary>
		public Boolean IsResize { get; set; }

		/// <summary>
		/// 成员类型
		/// </summary>
		[Required]
		public String MemberType { get; set; }

		/// <summary>
		/// 桌面索引
		/// </summary>
		public Int32 DeskIndex { get; set; }

		/// <summary>
		/// 账户Id
		/// </summary>
		public Int32 AccountId { get; set; }

		public Boolean IsIconByUpload { get; set; }

	}
}
