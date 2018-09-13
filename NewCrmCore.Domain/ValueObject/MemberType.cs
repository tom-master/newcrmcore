using System.ComponentModel;

namespace NewCrmCore.Domain.ValueObject
{
    /// <summary>
    /// 桌面应用类型
    /// </summary>
    public enum MemberType
    {
        [Description("应用")]
        App = 1,

        [Description("文件夹")]
        Folder = 2,

        [Description("挂件")]
        Widget = 3
    }
}
