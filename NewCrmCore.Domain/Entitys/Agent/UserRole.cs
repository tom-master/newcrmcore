using System;
using NewLibCore.Data.SQL.Mapper.Extension;
using NewLibCore.Data.SQL.Mapper.Extension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.Agent
{
    public class UserRole : EntityBase
    {
        [Required]
        public Int32 UserId { get; private set; }

        [Required]
        public Int32 RoleId { get; private set; }

        public UserRole(Int32 userId, Int32 roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public UserRole() { }
    }
}
