using System;
using System.Collections.Generic;

namespace NewCrmCore.Dto
{
    public sealed class DeskDto : BaseDto
    {
        public Int32 AccountId { get; set; }

        public Int32 DeskNumber { get; set; }

        public IList<MemberDto> Members { get; set; }
    }
}
  