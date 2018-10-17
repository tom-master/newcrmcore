using System;

namespace NewCrmCore.Dto
{
    public class VisitorRecordDto : BaseDto
    {
        public Int32 UserId { get; set; }

        public String UserName { get; set; }

        public String Controller { get; set; }

        public String Action { get; set; }

        public String Ip { get; set; }

        public String VisitorUrl { get; set; }

        public String UrlParameter { get; set; }
    }
}