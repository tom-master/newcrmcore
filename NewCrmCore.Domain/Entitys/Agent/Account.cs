using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewCRM.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCRM.Domain.Entitys.Agent
{
	[Description("用户"), Serializable]
	public partial class Account: DomainModelBase
	{
		private String _name;

		private String _loginPassword;

		private String _lockScreenPassword;

		private Boolean _isDisable;

		private DateTime _lastLoginTime;

		private Boolean _isOnline;

		private Boolean _isAdmin;

		private Int32 _configId;

		private IEnumerable<AccountRole> _roles;

		/// <summary>
		/// 用户名
		/// </summary>
		[PropertyRequired, InputRange(6)]
		public String Name
		{
			get
			{
				return _name;
			}
			private set
			{
				_name = value;
			}
		}

		/// <summary>
		/// 登陆密码
		/// </summary>
		[PropertyRequired, InputRange(6, 8)]
		public String LoginPassword
		{
			get
			{
				return _loginPassword;
			}
			private set
			{
				_loginPassword = value;
			}
		}

		/// <summary>
		/// 锁屏密码
		/// </summary>
		[PropertyRequired, InputRange(6, 8)]
		public String LockScreenPassword
		{
			get
			{
				return _lockScreenPassword;
			}
			private set
			{
				_lockScreenPassword = value;
			}
		}

		/// <summary>
		/// 是否禁用
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsDisable
		{
			get
			{
				return _isDisable;
			}
			private set
			{
				_isDisable = value;
			}
		}

		/// <summary>
		/// 最后一次登录的时间
		/// </summary>
		[DateTimeDefaultValue]
		public DateTime LastLoginTime
		{
			get
			{
				return _lastLoginTime;
			}
			private set
			{
				_lastLoginTime = value;
			}
		}

		/// <summary>
		/// 是否在线
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsOnline
		{
			get
			{
				return _isOnline;
			}
			private set
			{
				_isOnline = value;
			}
		}

		/// <summary>
		/// 是否为管理员
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsAdmin
		{
			get
			{
				return _isAdmin;
			}
			private set
			{
				_isAdmin = value;
			}
		}

		/// <summary>
		/// 配置Id
		/// </summary>
		[PropertyRequired]
		public Int32 ConfigId
		{
			get
			{
				return _configId;
			}
			private set
			{
				_configId = value;
			}
		}

		public String AccountFace { get; set; }

		/// <summary>
		/// 用户角色
		/// </summary>
		public IEnumerable<AccountRole> Roles
		{
			get
			{
				return _roles;
			}
			private set
			{
				_roles = value;
			}
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
			OnPropertyChanged(nameof(LoginPassword));
			return this;
		}

		public Account ModifyLockScreenPassword(String password)
		{
			if (String.IsNullOrEmpty(password))
			{
				throw new ArgumentException($@"{nameof(LockScreenPassword)} is null");
			}

			LockScreenPassword = password;
			OnPropertyChanged(nameof(LockScreenPassword));
			return this;
		}

		public Account Enable()
		{
			IsDisable = false;
			OnPropertyChanged(nameof(Enable));
			return this;
		}

		public Account Disable()
		{
			IsDisable = true;
			OnPropertyChanged(nameof(IsDisable));
			return this;
		}

		public Account Online()
		{
			IsOnline = true;
			LastLoginTime = DateTime.Now;
			OnPropertyChanged(nameof(IsOnline), nameof(LastLoginTime));
			return this;
		}

		public Account Offline()
		{
			IsOnline = false;
			OnPropertyChanged(nameof(IsOnline));
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
			OnPropertyChanged(nameof(Roles));
			return this;
		}
	}
}
