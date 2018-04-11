using System;
using System.ComponentModel.DataAnnotations;

namespace NewCrmCore.Dto
{
    public sealed class AppStyleDto : BaseDto
    {
        public String Name { get; set; }

        public String Type { get; set; }
    }
}
