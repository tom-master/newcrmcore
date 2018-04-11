using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCRM.Domain.Entitys.System;

namespace NewCRM.Domain.Services.Interface
{
	public interface ILoggerContext
	{
		/// <summary>
		/// 添加日志
		/// </summary>
		Task AddLoggerAsync(Log log);

		/// <summary>
		/// 获取日志列表
		/// </summary>
		/// <returns></returns>
		IList<Log> GetLogs(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);
	}
}
