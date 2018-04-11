using System;
using System.ComponentModel.DataAnnotations;

namespace NewCrmCore.Dto
{
    public sealed class AppDto : BaseDto
    {
        #region public property

        /// <summary>
        /// 名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        public String IconUrl { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// 使用人数
        /// </summary>
        public Int32 UseCount { get; set; }

        /// <summary>
        /// 评价星级
        /// </summary>
        public Double StartCount { get; set; }

        /// <summary>
        /// 是否安装
        /// </summary>
        public Boolean IsInstall { get; set; }

        /// <summary>
        /// 开发者（用户）Id
        /// </summary>
        public Int32 AccountId { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public Int32 AppAuditState { get; set; }

        ///// <summary>
        ///// 发布状态
        ///// </summary>
        public Int32 AppReleaseState { get; set; }

        /// <summary>
        /// app类型Id
        /// </summary>
        public Int32 AppTypeId { get; set; }

        /// <summary>
        /// app样式
        /// </summary>
        public Int32 AppStyle { get; set; }

        /// <summary>
        /// app分类
        /// </summary>
        public String AppTypeName { get; set; }

        public String AddTime { get; set; }

        public Boolean IsResize { get; set; }

        public Boolean IsOpenMax { get; set; }

        public Boolean IsFlash { get; set; }

        public String AppUrl { get; set; }

        public Int32 Width { get; set; }

        public Int32 Height { get; set; }

        public Boolean IsRecommand { get; set; }

        public Boolean IsCreater { get; set; }

        public String AccountName { get; set; }

        public Boolean IsIconByUpload { get; set; }

        #endregion
    }
}
