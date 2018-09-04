using System;
using System.ComponentModel;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    [Serializable, Description("壁纸")]
    public partial class Wallpaper : DomainModelBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        [PropertyRequired, InputRange(15)]
        public String Title { get; private set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [PropertyRequired, InputRange(150)]
        public String Url { get; private set; }

        /// <summary>
        /// 短地址
        /// </summary> 
        [PropertyDefaultValue(typeof(String), "")]
        public String ShortUrl { get; private set; }

        /// <summary>
        /// 来源
        /// </summary>
        [PropertyRequired]
        public WallpaperSource Source { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        [PropertyDefaultValue(typeof(String), ""), InputRange(50)]
        public String Description { get; private set; }

        /// <summary>
        /// 图片的宽
        /// </summary>
        [PropertyRequired]
        public Int32 Width { get; private set; }

        /// <summary>
        /// 图片的高
        /// </summary>
        [PropertyRequired]
        public Int32 Height { get; private set; }

        /// <summary>
        /// 上传者（用户）
        /// </summary>
        [PropertyRequired]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// md5
        /// </summary>
        [PropertyRequired]
        public String Md5 { get; private set; }

        /// <summary>
        /// 实例化一个壁纸对象
        /// </summary>
        public Wallpaper(String title, String url, String description, Int32 width, Int32 height, String md5, Int32 userId = default(Int32), WallpaperSource wallpaperSource = default(WallpaperSource))
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
