using System;
using System.ComponentModel;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    [Description("日志"), Serializable]
    public partial class Log : DomainModelBase
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        [PropertyDefaultValue(typeof(LogLevel), LogLevel.Info)]
        public LogLevel LogLevelEnum { get; private set; }

        /// <summary>
        /// 类名
        /// </summary>
        [PropertyRequired, InputRange(25)]
        public String Controller { get; private set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [PropertyRequired, InputRange(30)]
        public String Action { get; private set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [PropertyRequired, InputRange(1000)]
        public String ExceptionMessage { get; private set; }

        /// <summary>
        /// 异常堆栈
        /// </summary>
        [PropertyRequired]
        public String Track { get; private set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [PropertyRequired]
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
