using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewCrmCore.Dto
{
    public sealed class UserDto : BaseDto
    {
        public String Name { get; set; }

        public Boolean IsOnline { get; set; }

        public String Password { get; set; }

        public String LockScreenPassword { get; set; }

        public Boolean IsAdmin { get; set; }

        public String UserFace { get; set; }

        public IList<RoleDto> Roles { get; set; }

        public Boolean IsDisable { get; set; }

        public String AddTime { get; set; }

        public String LastLoginTime { get; set; }

        public String LastModifyTime { get; set; }

        public Boolean IsRememberPasswrod { get; set; }

        public Boolean IsModifyUserFace { get; set; }

        public UserDto()
        {
            Roles = new List<RoleDto>();
        }
    }

    public class LoginParameter
    {
        public String Name { get; set; }

        public String Password { get; set; }

        public Boolean IsRememberPasswrod { get; set; }
    }
}
