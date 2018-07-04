﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.Security
{
	[Description("角色"), Serializable]
	public partial class Role : DomainModelBase
	{


		#region public property

		/// <summary>
		/// 名称
		/// </summary>
		[PropertyRequired, InputRange(2,10)]
		public String Name
		{
			get; private set;
		}
		/// <summary>
		/// 角色标识
		/// </summary>
		[PropertyRequired, InputRange(2,20)]
		public String RoleIdentity
		{
			get; private set;
		}

		/// <summary>
		/// 备注
		/// </summary>
		[InputRange(50), PropertyDefaultValue(typeof(String), "")]
		public String Remark
		{
			get; private set;
		}

		/// <summary>
		/// 是否允许禁用
		/// </summary>
		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsAllowDisable
		{
			get; private set;
		}

		public IEnumerable<RolePower> Powers
		{
			get; private set;
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
			OnPropertyChanged(new PropertyArgs(nameof(Name), Name));
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
			OnPropertyChanged(new PropertyArgs(nameof(Powers), Powers));
			return this;
		}
	}
}
