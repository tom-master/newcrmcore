using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    public partial class VisitorRecord : DomainModelBase
    {
        [PropertyRequired]
        public Int32 UserId { get; private set; }

        [PropertyRequired, PropertyInputRange(10)]
        public String UserName { get; private set; }

        [PropertyRequired, PropertyInputRange(25)]
        public String Controller { get; private set; }

        [PropertyRequired, PropertyInputRange(30)]
        public String Action { get; private set; }

        [PropertyRequired]
        public String Ip { get; private set; }

        [PropertyRequired, PropertyInputRange(150)]
        public String VisitorUrl { get; private set; }

        [PropertyRequired, PropertyInputRange(150)]
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
    }
}