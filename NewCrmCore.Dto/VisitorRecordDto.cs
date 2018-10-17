using System;

namespace NewCrmCore.Dto
{
    public class VisitorRecordDto : BaseDto
    {
        public Int32 UserId { get; private set; }

        public String UserName { get; private set; }

        public String Controller { get; private set; }

        public String Action { get; private set; }

        public String Ip { get; private set; }

        public String VisitorUrl { get; private set; }

        public String UrlParameter { get; private set; }
    }
}