using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.Agent
{
	public class AccountRole: DomainModelBase
	{ 
		#region public property
		[PropertyRequired]
		public Int32 AccountId
		{
			get; private set;
		}

		[PropertyRequired]
		public Int32 RoleId
		{
			get; private set;
		}

		#endregion

		#region ctor

		public AccountRole(Int32 accountId, Int32 roleId)
		{
			AccountId = accountId;
			RoleId = roleId;
		}

		public AccountRole() { }

		#endregion
	}
}
