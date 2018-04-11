using System.ComponentModel;

namespace NewCRM.Domain.ValueObject
{
    /// <summary>
    /// 应用排列方向
    /// </summary>
    public enum AppAlignMode
    {
        [Description("横向排列 X轴")]
        X = 1,
        [Description("纵向排列 Y轴")]
        Y = 2
    }
}
