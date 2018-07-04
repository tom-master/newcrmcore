using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.Agent
{
	[Description("用户"), Serializable]
	public partial class Account : DomainModelBase
	{
		/// <summary>
		/// 用户名
		/// </summary>
		[PropertyRequired, InputRange(10)]
		public String Name
		{
			get; private set;
		}

		/// <summary>
		/// 登陆密码
		/// </summary>
		[PropertyRequired, InputRange(6, 8)]
		public String LoginPassword
		{
			get; private set;
		}

		/// <summary>
		/// 锁屏密码
		/// </summary>
		[PropertyRequired, InputRange(6, 8)]
		public String LockScreenPassword
		{
			get; private set;
		}

		/// <summary>
		/// 是否禁用
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsDisable
		{
			get; private set;
		}

		/// <summary>
		/// 最后一次登录的时间
		/// </summary>
		[DateTimeDefaultValue]
		public DateTime LastLoginTime
		{
			get; private set;
		}

		/// <summary>
		/// 是否在线
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsOnline
		{
			get; private set;
		}

		/// <summary>
		/// 是否为管理员
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsAdmin
		{
			get; private set;
		}

		/// <summary>
		/// 配置Id
		/// </summary>
		[PropertyRequired]
		public Int32 ConfigId
		{
			get; private set;
		}

		public String AccountFace
		{
			get; private set;
		}

		/// <summary>
		/// 用户角色
		/// </summary>
		public IEnumerable<AccountRole> Roles
		{
			get; private set;
		}

		#region ctor

		/// <summary>
		/// 实例化一个用户对象
		/// </summary>
		public Account(String name, String password, IEnumerable<AccountRole> roles, AccountType accountType = default(AccountType))
		{
			Name = name;
			LoginPassword = password;
			IsDisable = false;
			LastLoginTime = DateTime.Now;
			LockScreenPassword = password;
			IsOnline = false;
			IsAdmin = accountType == AccountType.Admin;
			Roles = roles;
		}

		public Account() { }

		#endregion
	}

	/// <summary>
	/// AccountExtension
	/// </summary>
	public partial class Account
	{
		public Account ModifyLoginPassword(String password)
		{
			if (String.IsNullOrEmpty(password))
			{
				throw new ArgumentException($@"{nameof(LoginPassword)} is null");
			}

			LoginPassword = password;
			OnPropertyChanged(new PropertyArgs(nameof(LoginPassword), password));
			return this;
		}

		public Account ModifyLockScreenPassword(String password)
		{
			if (String.IsNullOrEmpty(password))
			{
				throw new ArgumentException($@"{nameof(LockScreenPassword)} is null");
			}

			LockScreenPassword = password;
			OnPropertyChanged(new PropertyArgs(nameof(LockScreenPassword), password));
			return this;
		}

		public Account Enable()
		{
			IsDisable = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsDisable), IsDisable));
			return this;
		}

		public Account Disable()
		{
			IsDisable = true;
			OnPropertyChanged(new PropertyArgs(nameof(IsDisable), IsDisable));
			return this;
		}

		public Account Online()
		{
			IsOnline = true;
			LastLoginTime = DateTime.Now;
			OnPropertyChanged(new PropertyArgs(nameof(IsOnline), IsOnline), new PropertyArgs(nameof(LastLoginTime), LastLoginTime));
			return this;
		}

		public Account Offline()
		{
			IsOnline = false;
			OnPropertyChanged(new PropertyArgs(nameof(IsOnline), IsOnline));
			return this;
		}

		public Account ModifyRoles(params Int32[] roleIds)
		{
			if (roleIds.Length == 0)
			{
				return this;
			}

			Roles.ToList().Clear();
			Roles = roleIds.Select(roleId => new AccountRole(Id, roleId));
			OnPropertyChanged(new PropertyArgs(nameof(Roles), Roles));
			return this;
		}
	}
}
