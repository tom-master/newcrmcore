using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.Security
{
	public class RolePower : DomainModelBase
	{
		[PropertyRequired]
		public Int32 RoleId { get; private set; }

		[PropertyRequired]
		public Int32 AppId { get; private set; }

		public RolePower(Int32 roleId, Int32 appId)
		{
			RoleId = roleId;
			AppId = appId;
		}

		public RolePower()
		{

		}
	}
}
