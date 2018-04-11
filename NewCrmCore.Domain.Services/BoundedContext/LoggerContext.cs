using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using NewCRM.Domain.Entitys.System;
using NewCRM.Domain.Services.Interface;
using NewLib.Data.Mapper.InternalDataStore;
using NewLib.Validate;

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

		public IList<Log> GetLogs(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
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

				#region totalCount
				{
					var sql = $@"SELECT COUNT(*) FROM dbo.Log AS a WHERE 1=1 {where}";
					totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
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
					return dataStore.Find<Log>(sql, parameters);
				}
				#endregion
			}
		}
	}
}
