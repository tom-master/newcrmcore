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
                var mapper = EntityMapper.CreateMapper();
                {
                    mapper.Add(log);
                }
            });
        }

        public List<Log> GetLogs(String userName, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userName, true);
            Parameter.Validate(logLevel);

            var mapper = EntityMapper.CreateMapper();
            {
                // var where = new StringBuilder();
                // var parameters = new List<EntityParameter>();
                var level = EnumExtensions.ToEnum<LogLevel>(logLevel);
                var logWhere = MergeFactory.Create<Log>(w => w.LogLevelEnum == level);

                var userWhere = MergeFactory.Create<User>();
                if (!String.IsNullOrEmpty(userName))
                {
                    userWhere.And(w => w.Name.Contains(userName));
                    // parameters.Add(new EntityParameter("@name", userName));
                    // where.Append($@" AND a1.Name LIKE CONCAT('%',@name,'%') ");
                }

                var combination = logWhere.Append(userWhere);

                #region totalCount 
                {
                    totalCount = mapper.Select<Log>().LeftJoin<User>((a, b) => a.UserId == b.Id).Where<User>(combination).Count();
                    // var sql = $@"SELECT COUNT(*) FROM Log AS a LEFT JOIN newcrm_user AS a1 ON a.UserId=a1.Id WHERE 1=1 {where}";
                    // totalCount = mapper.ExecuteToSingle<Int32>(sql, parameters);
                }
                #endregion

                #region sql 
                {

                    return mapper.Select<Log>(a => new
                    {
                        a.LogLevelEnum,
                        a.Controller,
                        a.Action,
                        a.ExceptionMessage,
                        a.UserId,
                        a.AddTime
                    }).LeftJoin<User>((a, b) => a.UserId == b.Id)
                    .Where<User>(combination)
                    .Page(pageIndex, pageSize)
                    .OrderByDesc<Log, DateTime>(a => a.AddTime)
                    .ToList();
                    // var sql = $@"SELECT
                    //             a.LogLevelEnum,
                    //             a.Controller,
                    //             a.Action,
                    //             a.ExceptionMessage,
                    //             IFNULL(a.UserId,0) AS UserId,
                    //             a.AddTime
                    //             FROM Log AS a 
                    //             LEFT JOIN newcrm_user AS a1 ON a.UserId=a1.Id
                    //             WHERE 1=1 {where} ORDER BY a.AddTime DESC LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    // return mapper.ExecuteToList<Log>(sql, parameters);
                }
                #endregion
            }
        }
    }
}
