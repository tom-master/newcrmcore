using System;
using System.Collections.Generic;
using System.Text;

namespace NewCrmCore.Infrastructure.CommonTools
{
	public class PagingModel<TModel>
	{
		public Int32 TotalCount { get; set; }

		public IList<TModel> Models { get; set; }
	}
}
