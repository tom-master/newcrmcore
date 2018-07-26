using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NewLibCore.Data.Mapper.MapperExtension;
namespace NewCrmCore.Domain.Entitys.System
{

	[Serializable, Description("消息")]
	public class Notify : DomainModelBase
	{
		[PropertyRequired, InputRange(4, 10)]
		public String Title { get; private set; }

		[PropertyRequired, InputRange(1, 255)]
		public String Content { get; private set; }

		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsNotity { get; private set; }

		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsRead { get; private set; }

		[PropertyRequired]
		public Int32 AccountId { get; private set; }

		[PropertyRequired]
		public Int32 ToAccountId { get; private set; }
	}
}
