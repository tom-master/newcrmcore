using System;

namespace NewCrmCore.Dto
{
    public class NotifyDto : BaseDto
    {
        public String Title { get; set; }

        public String Content { get; set; }

        public Boolean IsNotify { get; set; }

        public Boolean IsRead { get; set; }

        public Int32 UserId { get; set; }

        public Int32 ToUserId { get; set; }
    }

}