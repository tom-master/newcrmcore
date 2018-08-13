using System;
using System.ComponentModel.DataAnnotations;
using NewCrmCore.Domain.ValueObject;
namespace NewCrmCore.Dto
{
    public sealed class AppDto : BaseDto
    {
        #region public property

        public String Name { get; set; }

        public String IconUrl { get; set; }

        public String Remark { get; set; }

        public Int32 UseCount { get; set; }

        public Double StarCount { get; set; }

        public Boolean IsInstall { get; set; }

        public Int32 AccountId { get; set; }

        public AppAuditState AppAuditState { get; set; }

        public AppReleaseState AppReleaseState { get; set; }

        public Int32 AppTypeId { get; set; }

        public AppStyle AppStyle { get; set; }

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
