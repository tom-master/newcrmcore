using System;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Data.SQL.Mapper.Validate;

namespace NewCrmCore.Domain.Entitys.System
{
    [TableName("newcrm_log")]
    public partial class Log : EntityBase
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        [DefaultValue(typeof(LogLevel), LogLevel.Info)]
        public LogLevel LogLevelEnum { get; private set; }

        /// <summary>
        /// 类名
        /// </summary>
        [Required, InputRange(25)]
        public String Controller { get; private set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [Required, InputRange(30)]
        public String Action { get; private set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [Required, InputRange(1000)]
        public String ExceptionMessage { get; private set; }

        /// <summary>
        /// 异常堆栈
        /// </summary>
        [Required]
        public String Track { get; private set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        public Log(Int32 userId, String controller, String action, LogLevel logLevel, String track, String exceptionMessage)
        {
            UserId = userId;
            Controller = controller;
            Action = action;
            LogLevelEnum = logLevel;
            Track = track;
            ExceptionMessage = exceptionMessage;
        }

        public Log()
        {

        }
    }
}
