using System;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
	public interface ILoggerServices
	{
		Task AddLoggerAsync(LogDto log);

		Task<PagingModel<LogDto>> GetLogsAsync(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize);
	}
}
