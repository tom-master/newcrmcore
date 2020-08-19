using System;
using System.Text.Json.Serialization;
using NewCrmCore.Domain.ValueObject;
namespace NewCrmCore.Dto
{
    public sealed class ConfigDto : BaseDto
    {
        public String Skin { get; set; }

        public String UserFace { get; set; }

        public Int32 AppSize { get; set; }

        public Int32 AppVerticalSpacing { get; set; }

        public Int32 AppHorizontalSpacing { get; set; }

        public Int32 DefaultDeskNumber { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppAlignMode AppXy { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DockPosition DockPosition { get; set; }

        public String WallpaperUrl { get; set; }

        public Int32 WallpaperWidth { get; set; }

        public Int32 WallpaperHeigth { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WallpaperSource WallpaperSource { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WallpaperMode WallpaperMode { get; set; }

        public Int32 DefaultDeskCount { get; set; }

        public Int32 UserId { get; set; }

        public Boolean IsBing { get; set; }

        public Boolean IsModifyUserFace { get; set; }

    }
}
