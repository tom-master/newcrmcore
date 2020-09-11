using System;
namespace NewCrmCore.Dto
{
    public sealed class CreateWindowDto : BaseDto
    {
        public String type { get; set; }
        public Int32 memberId { get; set; }
        public Int32 appId { get; set; }
        public String name { get; set; }
        public String icon { get; set; }
        public Int32 width { get; set; }
        public Int32 height { get; set; }
        public Boolean isOnDock { get; set; }
        public Boolean isOpenMax { get; set; }
        public Boolean isSetbar { get; set; }
        public String url { get; set; }
        public Boolean isResize { get; set; }
        public Double starCount { get; set; }

    }
}