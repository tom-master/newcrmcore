using System;
using System.Text.Json.Serialization;
using NewCrmCore.Domain.ValueObject;

namespace NewCrmCore.Dto
{
    public sealed class LogDto : BaseDto
    {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel LogLevelEnum { get; set; }

        public String Controller { get; set; }

        public String Action { get; set; }

        public String ExceptionMessage { get; set; }

        public String Track { get; set; }

        public Int32 UserId { get; set; }

        public String AddTime { get; set; }
    }
}
