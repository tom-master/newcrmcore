using System;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Validate;

namespace NewCrmCore.Application.Services
{
    public class LoggerServices : ILoggerServices
    {
        private readonly ILoggerContext _loggerContext;

        public LoggerServices(ILoggerContext loggerContext)
        {
            _loggerContext = loggerContext;
        }

        public async Task AddLoggerAsync(LogDto log)
        {
            Parameter.Validate(log);
            await _loggerContext.AddLoggerAsync(log.ConvertToModel<LogDto, Log>());
        }

        public async Task<PageList<LogDto>> GetLogsAsync(String userName, Int32 logLevel, Int32 pageIndex, Int32 pageSize)
        {
            var result = _loggerContext.GetLogs(userName, logLevel, pageIndex, pageSize, out var totalCount);
            return await Task.Run(() =>
            {
                return new PageList<LogDto>
                {
                    TotalCount = totalCount,
                    Models = result.Select(s => new LogDto
                    {
                        UserId = s.UserId,
                        Action = s.Action,
                        AddTime = s.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Controller = s.Controller,
                        ExceptionMessage = s.ExceptionMessage,
                        Id = s.Id,
                        LogLevelEnum = s.LogLevelEnum
                    }).ToList()
                };
            });
        }
    }
}
