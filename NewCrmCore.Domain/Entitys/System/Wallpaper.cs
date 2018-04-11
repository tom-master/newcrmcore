using System;
using System.ComponentModel;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
	[Serializable, Description("壁纸")]
	public partial class Wallpaper : DomainModelBase
	{
		#region private field

		private String _title;

		private String _url;

		private String _shortUrl;

		private WallpaperSource _source;

		private String _description;

		private Int32 _width;

		private Int32 _height;

		private Int32 _accountId;

		private String _md5;
		#endregion

		#region public property

		/// <summary>
		/// 标题
		/// </summary>
		[PropertyRequired]
		public String Title
		{
			get { return _title; }
			private set
			{
				_title = value;
			}
		}

		/// <summary>
		/// 图片地址
		/// </summary>
		[PropertyRequired]
		public String Url
		{
			get { return _url; }
			private set
			{
				_url = value;
			}
		}

		/// <summary>
		/// 短地址
		/// </summary> 
		[PropertyDefaultValue(typeof(String), "")]
		public String ShortUrl
		{
			get { return _shortUrl; }
			private set
			{
				_shortUrl = value;
			}
		}

		/// <summary>
		/// 来源
		/// </summary>
		[PropertyRequired]
		public WallpaperSource Source
		{
			get { return _source; }
			private set
			{
				_source = value;
			}
		}

		/// <summary>
		/// 描述
		/// </summary>
		[PropertyDefaultValue(typeof(String), ""), InputRange(50)]
		public String Description
		{
			get { return _description; }
			private set
			{
				_description = value;
			}
		}

		/// <summary>
		/// 图片的宽
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
		/// 图片的高
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
		/// 上传者（用户）
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
		/// md5
		/// </summary>
		[PropertyRequired]
		public String Md5
		{
			get { return _md5; }
			private set
			{
				_md5 = value;
			}
		}

		#endregion

		#region ctor

		/// <summary>
		/// 实例化一个壁纸对象
		/// </summary>
		public Wallpaper(String title, String url, String description, Int32 width, Int32 height, String md5, Int32 accountId = default(Int32), WallpaperSource wallpaperSource = default(WallpaperSource))
		{
			Title = title;
			Url = url;
			Description = description;
			Width = width;
			Height = height;
			Source = wallpaperSource;
			AccountId = accountId;
			Md5 = md5;
		}

		public Wallpaper()
		{

		}

		#endregion
	}
}
