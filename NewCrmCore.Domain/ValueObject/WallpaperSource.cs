using System.ComponentModel;

namespace NewCrmCore.Domain.ValueObject
{
    /// <summary>
    /// 壁纸来源
    /// </summary>
    public enum WallpaperSource
    {
        [Description("系统")]
        System = 1,

        [Description("网络")]
        Web = 2,

        [Description("上传")]
        Upload = 3,

        [Description("Bing")]
        Bing=4

    }
}
