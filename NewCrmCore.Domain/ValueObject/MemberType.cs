using System.ComponentModel;

namespace NewCRM.Domain.ValueObject
{
    /// <summary>
    /// 桌面成员类型
    /// </summary>
    public enum MemberType
    {
        [Description("应用")]
        App = 1,
        [Description("文件夹")]
        Folder = 2
    }
}
