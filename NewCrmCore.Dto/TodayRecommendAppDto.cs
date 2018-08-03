using System;
using NewCrmCore.Domain.ValueObject;

namespace NewCrmCore.Dto
{
    public sealed class TodayRecommendAppDto : BaseDto
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 使用数量
        /// </summary>
        public Int32 UseCount { get; set; }

        /// <summary>
        /// 应用图标
        /// </summary>
        public String AppIcon { get; set; }

        /// <summary>
        /// 应用评价
        /// </summary>
        public Double AppStars { get; set; }

        /// <summary>
        /// 是否安装
        /// </summary>
        public Boolean IsInstall { get; set; }

        /// <summary>
        /// 应用说明
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// 应用样式
        /// </summary>
        public AppStyle Style { get; set; }

        /// <summary>
        /// 图标是否来自上传
        /// </summary>
        public Boolean IsIconByUpload { get; set; }
    }
}
