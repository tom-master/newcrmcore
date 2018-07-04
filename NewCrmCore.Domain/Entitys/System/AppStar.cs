using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
	public partial class AppStar : DomainModelBase
	{


		/// <summary>
		/// 账号
		/// </summary>
		[PropertyRequired]
		public Int32 AccountId
		{
			get; private set;
		}

		/// <summary>
		/// 应用Id
		/// </summary>
		[PropertyRequired]
		public Int32 AppId
		{
			get; private set;
		}

		/// <summary>
		/// 评分
		/// </summary>
		[PropertyRequired]
		public Double StartNum
		{
			get; private set;
		}

		public AppStar(Int32 accountId, Int32 appId, Double startNum)
		{
			AccountId = accountId;
			StartNum = startNum;
			AppId = appId;
		}

		public AppStar()
		{
		}
	}
}
