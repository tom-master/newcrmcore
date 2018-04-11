using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.Security
{
	public class RolePower: DomainModelBase
	{
		#region private field
		private Int32 _roleId;

		private Int32 _appId;
		#endregion

		#region public property
		[PropertyRequired]
		public Int32 RoleId
		{
			get { return _roleId; }
			private set
			{
				_roleId = value;
			}
		}

		[PropertyRequired]
		public Int32 AppId
		{
			get { return _appId; }
			private set
			{
				_appId = value;
			}
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
