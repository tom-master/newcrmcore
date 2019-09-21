using System;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.SQL.Mapper;

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

        public AppStyle Style { get; set; }

        public Boolean IsIconByUpload { get; set; }
    }
}
