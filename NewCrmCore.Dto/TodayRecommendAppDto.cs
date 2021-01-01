using System;
using System.Text.Json.Serialization;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Storage.SQL;

namespace NewCrmCore.Dto
{
    public sealed class TodayRecommendAppDto : EntityBase
    {

        public String Name { get; set; }

        public Int32 UseCount { get; set; }

        public String AppIcon { get; set; }

        public Double AppStars { get; set; }

        public Boolean IsInstall { get; set; }

        public String Remark { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppStyle Style { get; set; }

        public Boolean IsIconByUpload { get; set; }
    }
}
