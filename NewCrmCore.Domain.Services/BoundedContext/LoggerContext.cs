using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewLibCore;
using NewLibCore.Storage.SQL;
using NewLibCore.Storage.SQL.Filter;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
{
    public class LoggerContext : ILoggerContext
    {
        public async Task AddLoggerAsync(Log log)
        {
            Parameter.IfNullOrZero(log);
            await Task.Run(() =>
            {
                using var mapper = EntityMapper.CreateMapper();
                try
                {
                    mapper.Add(log);
                }
                catch (System.Exception)
                {
                    throw;
                }
            });
        }

        public List<Log> GetLogs(String userName, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.IfNullOrZero(userName, true);
            Parameter.IfNullOrZero(logLevel);

            try
            {
                using var mapper = EntityMapper.CreateMapper();
                try
                {


                    var level = EnumExtensions.ToEnum<LogLevel>(logLevel);
                    var logWhere = FilterFactory.Create<Log>(w => w.LogLevelEnum == level);

                    var userWhere = FilterFactory.Create<User>();
                    if (!String.IsNullOrEmpty(userName))
                    {
                        userWhere.And(w => w.Name.Contains(userName));
                    }

                    var filter = logWhere.Append(userWhere);

                    #region totalCount 
                    {
                        totalCount = mapper.Query<Log>()
                        .LeftJoin<User>((a, b) => a.UserId == b.Id)
                        .Where<User>(filter).Count();
                    }
                    #endregion

                    #region sql 
                    {
                        return mapper.Query<Log>().LeftJoin<User>((a, b) => a.UserId == b.Id)
                        .Where<User>(filter)
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
                catch (System.Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
