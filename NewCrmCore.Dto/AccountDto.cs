using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewCrmCore.Dto
{
	public sealed class AccountDto: BaseDto
	{
		/// <summary>
		/// 用户名
		/// </summary>
		public String Name { get; set; }

		/// <summary>
		/// 是否在线
		/// </summary>
		public Boolean IsOnline { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		public String Password { get; set; }

		/// <summary>
		/// 锁屏密码
		/// </summary>
		public String LockScreenPassword { get; set; }

		/// <summary>
		/// 用户类型
		/// </summary>
		public Boolean IsAdmin { get; set; }

		/// <summary>
		/// 头像
		/// </summary>
		public String AccountFace { get; set; }

		/// <summary>
		/// 用户角色Id
		/// </summary>
		public List<RoleDto> Roles { get; set; }

		/// <summary>
		/// 是否禁用
		/// </summary>
		public Boolean IsDisable { get; set; }

		public String AddTime { get; set; }

		public String LastLoginTime { get; set; }

		public String LastModifyTime { get; set; }

		public Boolean IsRememberPasswrod { get; set; }
	}

	public class LoginParameter
	{
		public String Name { get; set; }

		public String Password { get; set; }

		public Boolean IsRememberPasswrod { get; set; }
	}
}
