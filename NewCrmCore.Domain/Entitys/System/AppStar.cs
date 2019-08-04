using System; 
using NewLibCore.Data.SQL.Mapper.EntityExtension;
using NewLibCore.Data.SQL.Mapper.EntityExtension.EntityAttribute;

namespace NewCrmCore.Domain.Entitys.System
{
    [TableName("newcrm_app_star")]
    public partial class AppStar : EntityBase
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        public Int32 UserId { get; private set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [Required]
        public Int32 AppId { get; private set; }

        /// <summary>
        /// 评分
        /// </summary>
        [Required]
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
