using System.ComponentModel;

namespace NewCRM.Domain.ValueObject
{
    /// <summary>
    /// 码头位置
    /// </summary>
    public enum DockPostion
    {
		/// <summary>
		/// 上
		/// </summary>
		[Description("上")]
        Top = 1,

		/// <summary>
		/// 左
		/// </summary>
		[Description("左")]
        Left = 2,

		/// <summary>
		/// 右
		/// </summary>
		[Description("右")]
        Right = 3,

		/// <summary>
		/// 停用
		/// </summary>
		[Description("停用")]
        None = 4
    }
}
