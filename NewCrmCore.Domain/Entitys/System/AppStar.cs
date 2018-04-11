using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCrmCore.Domain.Entitys.System
{
	public partial class AppStar : DomainModelBase
	{
		private Int32 _accountId;

		public Int32 _appId;

		public Double _startNum;

		/// <summary>
		/// 账号
		/// </summary>
		[PropertyRequired]
		public Int32 AccountId
		{
			get { return _accountId; }
			private set
			{
				_accountId = value;
			}
		}

		/// <summary>
		/// 应用Id
		/// </summary>
		[PropertyRequired]
		public Int32 AppId
		{
			get { return _appId; }
			private set
			{
				_appId = value;
			}
		}

		/// <summary>
		/// 评分
		/// </summary>
		[PropertyRequired]
		public Double StartNum
		{
			get { return _startNum; }
			private set
			{
				_startNum = value;
			}
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
