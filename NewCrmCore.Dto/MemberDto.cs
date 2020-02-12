using System;
using NewCrmCore.Domain.ValueObject;

namespace NewCrmCore.Dto
{
    public sealed class MemberDto : BaseDto
    {
        public Int32 AppId { get; set; }

        public Int32 Width { get; set; }

        public Int32 Height { get; set; }

        public Int32 FolderId { get; set; }

        public String Name { get; set; }

        public String IconUrl { get; set; }

        public String AppUrl { get; set; }

        public Boolean IsOnDock { get; set; }

        public Boolean IsSetbar { get; set; }

        public Boolean IsOpenMax { get; set; }

        public Boolean IsFlash { get; set; }

        public Boolean IsResize { get; set; }

        public MemberType MemberType { get; set; }

        public Int32 DeskIndex { get; set; }

        public Int32 UserId { get; set; }

        public Boolean IsIconByUpload { get; set; }

        public Double StarCount { get; set; }

    }
}
