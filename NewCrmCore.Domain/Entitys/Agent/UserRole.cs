using System;
using NewLibCore.Data.SQL.MapperExtension;
using NewLibCore.Data.SQL.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.Agent
{
    public class UserRole : DomainModelBase
    {
        [PropertyRequired]
        public Int32 UserId { get; private set; }

        [PropertyRequired]
        public Int32 RoleId { get; private set; }

        public UserRole(Int32 userId, Int32 roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public UserRole() { }
    }
}
