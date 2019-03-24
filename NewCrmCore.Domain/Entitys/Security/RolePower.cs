using System;
using NewLibCore.Data.SQL.MapperExtension;
using NewLibCore.Data.SQL.MapperExtension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.Security
{
	public class RolePower : DomainModelBase
	{
		[Required]
		public Int32 RoleId { get; private set; }

		[Required]
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
