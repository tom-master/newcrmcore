using System;
using NewCrmCore.Domain.ValueObject;
namespace NewCrmCore.Dto
{
	public sealed class WallpaperDto: BaseDto
	{
		/// <summary>
		/// 账户Id
		/// </summary>
		public Int32 AccountId { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		public String Title { get; set; }

		/// <summary>
		/// 图片地址
		/// </summary>
		public String Url { get; set; }

		/// <summary>
		/// 短URL
		/// </summary>
		public String ShortUrl { get; set; }

		/// <summary>
		/// 来源
		/// </summary>
		public WallpaperSource Source { get; set; }

		/// <summary>
		/// 图片的宽
		/// </summary>
		public Int32 Width { get; set; }

		/// <summary>
		/// 图片的高
		/// </summary>
		public Int32 Height { get; set; }

		/// <summary>
		/// 图片MD5
		/// </summary>
		public String Md5 { get; set; }

	}
}
