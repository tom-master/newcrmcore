using System;
using NewLibCore.Data.SQL.MapperExtension;
using NewLibCore.Data.SQL.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    public partial class AppStar : DomainModelBase
    {
        /// <summary>
        /// 账号
        /// </summary>
        [PropertyRequired]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [PropertyRequired]
        public Int32 AppId { get; private set; }

        /// <summary>
        /// 评分
        /// </summary>
        [PropertyRequired]
        public Double StartNum { get; private set; }

        public AppStar(Int32 userId, Int32 appId, Double startNum)
        {
            UserId = userId;
            StartNum = startNum;
            AppId = appId;
        }

        public AppStar()
        {
        }
    }
}
