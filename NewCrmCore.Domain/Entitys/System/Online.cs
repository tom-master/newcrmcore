using System;
using NewLibCore.Data.SQL.Mapper.Extension;
using NewLibCore.Data.SQL.Mapper.Extension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    public partial class Online : EntityBase
    {
        /// <summary>
        /// Ip地址
        /// </summary>
        [Required]
        public String IpAddress { get; private set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// 实例化一个在线状态的对象
        /// </summary>
        public Online(String ipAddress, Int32 userId)
        {
            IpAddress = ipAddress;
            UserId = userId;
        }

        /// <summary>
        /// 实例化一个在线状态的对象
        /// </summary>
        public Online()
        {
        }
    }
}
