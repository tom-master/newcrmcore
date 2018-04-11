using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCRM.Domain.Entitys.Security
{
	[Description("角色"), Serializable]
	public partial class Role: DomainModelBase
	{
		#region private field

		private String _name;

		private String _roleIdentity;

		private String _remark;

		private Boolean _isAllowDisable;

		private IEnumerable<RolePower> _powers;

		#endregion

		#region public property

		/// <summary>
		/// 名称
		/// </summary>
		[PropertyRequired, InputRange(6)]
		public String Name
		{
			get { return _name; }
			private set
			{
				_name = value;
			}
		}

		/// <summary>
		/// 角色标识
		/// </summary>
		[PropertyRequired, InputRange(10)]
		public String RoleIdentity
		{
			get { return _roleIdentity; }
			private set
			{
				_roleIdentity = value;
			}
		}

		/// <summary>
		/// 备注
		/// </summary>
		[InputRange(20), PropertyDefaultValue(typeof(String), "")]
		public String Remark
		{
			get { return _remark; }
			private set
			{
				_remark = value;
			}
		}

		/// <summary>
		/// 是否允许禁用
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsAllowDisable
		{
			get { return _isAllowDisable; }
			private set
			{
				_isAllowDisable = value;
			}
		}

		public IEnumerable<RolePower> Powers
		{
			get { return _powers; }
			private set
			{
				_powers = value;
			}
		}

		#endregion

		#region ctor
		/// <summary>
		/// 实例化一个角色对象
		/// </summary>
		public Role(String name, String roleIdentity, String remark = default(String), Boolean isAllowDisable = default(Boolean))
		{
			Name = name;
			Remark = remark;
			RoleIdentity = roleIdentity;
			IsAllowDisable = isAllowDisable;
			Powers = new List<RolePower>();
		}

		/// <summary>
		/// 实例化一个角色对象
		/// </summary>
		public Role()
		{
		}

		#endregion
	}

	/// <summary>
	/// RoleExtension
	/// </summary>
	public partial class Role
	{
		public Role ModifyRoleName(String roleName)
		{
			if (String.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException($@"{nameof(roleName)} is null");
			}

			Name = roleName;
			OnPropertyChanged(nameof(Name));
			return this;
		}

		public Role ModifyRolePower(params Int32[] appIds)
		{
			if (appIds.Length == 0)
			{
				return this;
			}

			Powers.ToList().Clear();
			Powers = appIds.Select(appId => new RolePower(Id, appId));
			OnPropertyChanged(nameof(Powers));
			return this;
		}
	}
}
