using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure;
using NewLibCore.Data.Mapper.InternalDataStore;
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
                using (var dataStore = new DataStore(Appsetting.Database))
                {
                    dataStore.Add(log);
                }
            });
        }

        public List<Log> GetLogs(String userName, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Parameter.Validate(userName, true);
            Parameter.Validate(logLevel);

            using (var dataStore = new DataStore(Appsetting.Database))
            {
                var where = new StringBuilder();
                var parameters = new List<ParameterMapper>();
                if (!String.IsNullOrEmpty(userName))
                {
                    parameters.Add(new ParameterMapper("@name", userName));
                    where.Append($@" AND a1.Name LIKE CONCAT('%',@name,'%') ");
                }

                #region totalCount 
                {
                    var sql = $@"SELECT COUNT(*) FROM Log AS a LEFT JOIN User AS a1 ON a.UserId=a1.Id WHERE 1=1 {where}";
                    totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
                }
                #endregion

                #region sql 
                {
                    var sql = $@"SELECT
                                a.LogLevelEnum,
                                a.Controller,
                                a.Action,
                                a.ExceptionMessage,
                                IFNULL(a.UserId,0) AS UserId,
                                a.AddTime
                                FROM Log AS a 
                                LEFT JOIN User AS a1 ON a.UserId=a1.Id
                                WHERE 1=1 {where} ORDER BY a.AddTime DESC LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
                    return dataStore.Find<Log>(sql, parameters);
                }
                #endregion
            }
        }
    }
}
