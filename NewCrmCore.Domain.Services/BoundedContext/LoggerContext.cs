using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewLibCore;
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Data.SQL.MergeExpression;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class LoggerContext : ILoggerContext
    {
        public async Task AddLoggerAsync(Log log)
        {
            Parameter.Validate(log);
            await Task.Run(() =>
            {
                using (var mapper = EntityMapper.CreateMapper())
                {
                    mapper.Add(log);
                }
            });
        }

        public List<Log> GetLogs(String userName, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userName, true);
            Parameter.Validate(logLevel);

            using (var mapper = EntityMapper.CreateMapper())
            {
                var level = EnumExtensions.ToEnum<LogLevel>(logLevel);
                var logWhere = MergeFactory.Create<Log>(w => w.LogLevelEnum == level);

                var userWhere = MergeFactory.Create<User>();
                if (!String.IsNullOrEmpty(userName))
                {
                    userWhere.And(w => w.Name.Contains(userName));
                }

                var combination = logWhere.Append(userWhere);

                #region totalCount 
                {
                    totalCount = mapper.Query<Log>()
                    .LeftJoin<User>((a, b) => a.UserId == b.Id)
                    .Where<User>(combination).Count();
                }
                #endregion

                #region sql 
                {
                    return mapper.Query<Log>().LeftJoin<User>((a, b) => a.UserId == b.Id)
                    .Where<User>(combination)
                    .Select(a => new
                    {
                        a.LogLevelEnum,
                        a.Controller,
                        a.Action,
                        a.ExceptionMessage,
                        a.UserId,
                        a.AddTime
                    })
                    .Page(pageIndex, pageSize)
                    .ThenByDesc<DateTime>(a => a.AddTime)
                    .ToList();
                }
                #endregion
            }
        }
    }
}
