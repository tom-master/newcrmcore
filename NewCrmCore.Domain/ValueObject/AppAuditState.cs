using System.ComponentModel;

namespace NewCRM.Domain.ValueObject
{
    /// <summary>
    /// App的审核状态
    /// </summary>
    public enum AppAuditState
    {
        [Description("暂不审核")]
        Wait = 1,
        [Description("通过")]
        Pass = 2,
        [Description("未通过")]
        Deny = 3,
        [Description("未审核")]
        UnAuditState = 4
    }
}
