using System.ComponentModel;

namespace NewCrmCore.Domain.ValueObject
{
    public enum UserType
    {
        [Description("普通用户")]
        User = 1,

        [Description("管理员")]
        Admin = 2
    }
}
