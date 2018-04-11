using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NewCRM.Domain.Entitys.System;
using NewCRM.Domain.Services.Interface;
using NewLib.Data.Mapper.InternalDataStore;
using NewLib.Validate;

namespace NewCRM.Domain.Services.BoundedContext
{
	public class SkinContext: ISkinContext
	{
		/// <summary>
		/// 修改皮肤
		/// </summary>
		public async Task ModifySkinAsync(Int32 accountId, String newSkin)
		{
			new Parameter().Validate(accountId).Validate(newSkin);
			await Task.Run(() =>
			{
				using (var dataStore = new DataStore())
				{
					var config = new Config();
					config.ModifySkin(newSkin);
					dataStore.ExecuteModify(config, conf => conf.AccountId == accountId);
				}
			});
		}
	}
}
