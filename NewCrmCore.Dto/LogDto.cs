using System;
using NewCrmCore.Domain.ValueObject;

namespace NewCrmCore.Dto
{
    public sealed class LogDto : BaseDto
    {

        public LogLevel LogLevelEnum { get; set; }

        public String Controller { get; set; }

        public String Action { get; set; }

        public String ExceptionMessage { get; set; }

        public String Track { get; set; }

        public Int32 AccountId { get; set; }

        public String AddTime { get; set; }
    }
}
