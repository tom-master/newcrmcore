using System; 
using NewLibCore.Data.SQL.Mapper.EntityExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    [TableName("newcrm_visitor_record")]
    public partial class VisitorRecord : EntityBase
    {
        [Required]
        public Int32 UserId { get; private set; }

        [Required, InputRange(10)]
        public String UserName { get; private set; }

        [Required, InputRange(25)]
        public String Controller { get; private set; }

        [Required, InputRange(30)]
        public String Action { get; private set; }

        [Required]
        public String Ip { get; private set; }

        [Required, InputRange(150)]
        public String VisitorUrl { get; private set; }

        [Required, InputRange(500)]
        public String UrlParameter { get; private set; }

        public VisitorRecord(Int32 userId, String userName, String controller, String action, String ip, String visitorUrl, String urlParameter)
        {
            UserId = userId;
            UserName = userName;
            Controller = controller;
            Action = action;
            Ip = ip;
            VisitorUrl = visitorUrl;
            UrlParameter = urlParameter;
        }

        public VisitorRecord() { }
    }
}