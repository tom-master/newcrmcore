using System;
using System.Text.Json.Serialization;
using NewCrmCore.Domain.ValueObject;
namespace NewCrmCore.Dto
{
    public sealed class WallpaperDto : BaseDto
    {
        public Int32 UserId { get; set; }

        public String Title { get; set; }

        public String Url { get; set; }

        public String ShortUrl { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public WallpaperSource Source { get; set; }

        public Int32 Width { get; set; }

        public Int32 Height { get; set; }

        public String Md5 { get; set; }

    }
}
