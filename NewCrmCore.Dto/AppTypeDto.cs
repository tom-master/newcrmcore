using System;
using System.ComponentModel.DataAnnotations;

namespace NewCrmCore.Dto
{
    public sealed class AppTypeDto : BaseDto
    {
        public String Name { get; set; }

        public Boolean IsSystem { get; set; }
    }
}
