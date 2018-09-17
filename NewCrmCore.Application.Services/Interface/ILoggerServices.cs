using System;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;

namespace NewCrmCore.Application.Services.Interface
{
    public interface ILoggerServices
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task AddLoggerAsync(LogDto log);

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="logLevel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageList<LogDto>> GetLogsAsync(String userName, Int32 logLevel, Int32 pageIndex, Int32 pageSize);
    }
}
