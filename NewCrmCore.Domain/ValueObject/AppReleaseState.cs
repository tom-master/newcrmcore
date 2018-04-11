using System.ComponentModel;

namespace NewCrmCore.Domain.ValueObject
{
    /// <summary>
    /// app发布状态
    /// </summary>
    public enum AppReleaseState
    {
        [Description("已发布")]
        Release = 1,
        [Description("未发布")]
        UnRelease = 2
    }
}
