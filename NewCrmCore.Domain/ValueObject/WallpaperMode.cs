using System.ComponentModel;

namespace NewCRM.Domain.ValueObject
{
    /// <summary>
    /// 显示方式
    /// </summary>
    public enum WallpaperMode
    {

		/// <summary>
		/// 填充
		/// </summary>
		[Description("填充")]
        Fill = 1,

		/// <summary>
		/// 适应
		/// </summary>
		[Description("适应")]
        Adapted = 2,

		/// <summary>
		/// 平铺
		/// </summary>
		[Description("平铺")]
        Tile = 3,

		/// <summary>
		/// 拉伸
		/// </summary>
		[Description("拉伸")]
        Draw = 4,

		/// <summary>
		/// 居中
		/// </summary>
		[Description("居中")]
        Center = 5
    }
}
