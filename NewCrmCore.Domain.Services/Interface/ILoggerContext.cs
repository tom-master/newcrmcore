using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Domain.Services.Interface
{
    public interface ILoggerContext
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task AddLoggerAsync(Log log);

        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="logLevel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Log> GetLogs(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);
    }
}
