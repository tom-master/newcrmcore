using System;
using System.Threading.Tasks;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure;
using NewLibCore.Data.Mapper.InternalDataStore;
using NewLibCore.Validate;

namespace NewCrmCore.Domain.Services.BoundedContext
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
				using (var dataStore = new DataStore(Appsetting.Database))
				{
					var config = new Config();
					config.ModifySkin(newSkin);
					dataStore.Modify(config, conf => conf.AccountId == accountId);
				}
			});
		}
	}
}
