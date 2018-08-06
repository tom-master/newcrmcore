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
			new Parameter().Validate(log);
			await Task.Run(() => 
			{
				using(var dataStore = new DataStore(Appsetting.Database)) 
				{
					dataStore.Add(log);
				}
			});
		}

		public List<Log> GetLogs(Int32 accountId, Int32 logLevel, Int32 pageIndex, Int32 pageSize, out Int32 totalCount) 
		{
			new Parameter().Validate(accountId).Validate(logLevel);

			using(var dataStore = new DataStore(Appsetting.Database)) 
			{
				var where = new StringBuilder();
				var parameters = new List<ParameterMapper>();
				if (accountId != 0)
				{
					parameters.Add(new ParameterMapper("AccountId", accountId));
					where.Append($@" AND a.AccountId=@AccountId");
				}
				
				#region totalCount 
				{
					var sql = $@"SELECT COUNT(*) FROM Log AS a WHERE 1=1 {where}";
					totalCount = dataStore.FindSingleValue<Int32>(sql, parameters);
				}
				#endregion

				#region sql 
				{
					var sql = $@" SELECT
											a.LogLevelEnum,
											a.Controller,
											a.Action,
											a.ExceptionMessage,
											a.Track
											FROM Log AS a WHERE 1=1 {where} LIMIT {pageSize * (pageIndex - 1)},{pageSize}";
					return dataStore.Find<Log>(sql, parameters);
				}
				#endregion
			 }
		}
	}
}
