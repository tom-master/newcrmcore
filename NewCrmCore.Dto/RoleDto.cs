using System;
using System.Collections.Generic;

namespace NewCrmCore.Dto
{
    public sealed class RoleDto : BaseDto
    {
        public String Name { get; set; }

        public String RoleIdentity { get; set; }

        public String Remark { get; set; }

        public IList<PowerDto> Powers { get; set; }

        public RoleDto()
        {
            Powers = new List<PowerDto>();

        }
    }
}
