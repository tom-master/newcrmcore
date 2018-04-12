using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCRM.Domain.Services.BoundedContext
{
	public class LoggerContext: ILoggerContext
	{
		public async Task AddLoggerAsync(Log log)
		{
			new Parameter().Validate(log);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					dataStore.ExecuteAdd(log);
				}
			});
		}

		public async Task<PagingModel<Log>> GetLogsAsync(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize)
		{
			new Parameter().Validate(accountId).Validate(logLevel);
			return await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var where = new StringBuilder();
					var parameters = new List<SqlParameter>();
					if (accountId != 0)
					{
						parameters.Add(new SqlParameter("AccountId", accountId));
						where.Append($@" AND a.AccountId=@AccountId");
					}

					var paging = new PagingModel<Log>();

					#region totalCount
					{
						var sql = $@"SELECT COUNT(*) FROM dbo.Log AS a WHERE 1=1 {where}";
						paging.TotalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
					}
					#endregion

					#region sql
					{
						var sql = $@"SELECT TOP (@pageSize) * FROM 
                                (
	                                SELECT
	                                ROW_NUMBER() OVER(ORDER BY a.Id DESC) AS rownumber, 
	                                a.LogLevelEnum,
	                                a.Controller,
	                                a.Action,
	                                a.ExceptionMessage,
	                                a.Track
	                                FROM dbo.Log AS a WHERE 1=1 {where}
                                ) AS aa WHERE aa.rownumber>@pageSize*(@pageIndex-1)";
						parameters.Add(new SqlParameter("@pageIndex", pageIndex));
						parameters.Add(new SqlParameter("@pageSize", pageSize));
						paging.Models = dataStore.Find<Log>(sql, parameters);
					}
					#endregion

					return paging;
				}
			});
		}
	}
}
