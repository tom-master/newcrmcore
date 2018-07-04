using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.Security
{
	public class RolePower: DomainModelBase
	{ 
		#region public property

		[PropertyRequired]
		public Int32 RoleId
		{
			get; private set;
		}

		[PropertyRequired]
		public Int32 AppId
		{
			get; private set;
		}

		#endregion

		#region ctor

		public RolePower(Int32 roleId, Int32 appId)
		{
			RoleId = roleId;
			AppId = appId;
		}

		public RolePower()
		{

		}

		#endregion
	}
}
