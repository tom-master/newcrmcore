using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Dto;

namespace NewCrmCore.Application.Services.Interface
{
	public interface ILoggerServices
	{
		Task AddLoggerAsync(LogDto log);

		IList<LogDto> GetLogs(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);
	}
}
