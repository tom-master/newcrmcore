using System;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.SQL.Mapper;

namespace NewCrmCore.Domain.Entitys.System
{
    [TableName("newcrm_wallpaper")]
    public partial class Wallpaper : EntityBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required, InputRange(15)]
        public String Title { get; private set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Required, InputRange(150)]
        public String Url { get; private set; }

        /// <summary>
        /// 短地址
        /// </summary> 
        [DefaultValue(typeof(String))]
        public String ShortUrl { get; private set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Required]
        public WallpaperSource Source { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DefaultValue(typeof(String)), InputRange(50)]
        public String Description { get; private set; }

        /// <summary>
        /// 图片的宽
        /// </summary>
        [Required]
        public Int32 Width { get; private set; }

        /// <summary>
        /// 图片的高
        /// </summary>
        [Required]
        public Int32 Height { get; private set; }

        /// <summary>
        /// 上传者（用户）
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// md5
        /// </summary>
        [Required]
        public String Md5 { get; private set; }

        /// <summary>
        /// 实例化一个壁纸对象
        /// </summary>
        public Wallpaper(String title, String url, String description, Int32 width, Int32 height, String md5, Int32 userId = 0, WallpaperSource wallpaperSource = default(WallpaperSource))
        {
            Title = title;
            Url = url;
            Description = description;
            Width = width;
            Height = height;
            Source = wallpaperSource;
            UserId = userId;
            Md5 = md5;
        }

        public Wallpaper()
        {

        }

    }
}
