using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.Redis.InternalHelper;

namespace NewCrmCore.Domain.Services
{

	public class CacheHelper
	{
		private static readonly ICacheQueryProvider _cacheQuery = new DefaultRedisQueryProvider(0,"");

		public static async Task<TModel> GetCache<TModel>(CacheKeyBase cache, Func<Task<TModel>> func) where TModel : class
		{
			var cts = new CancellationTokenSource(cache.CancelToken);

			TModel cacheResult = null;
			try
			{
				cacheResult = await Task.Run(() => _cacheQuery.StringGetAsync<TModel>(cache.GetKey()), cts.Token);
			}
			catch (OperationCanceledException)
			{

			}

			if (cacheResult != null)
			{
				return cacheResult;
			}

			var dbResult = await func();
			_cacheQuery.StringSet(cache.GetKey(), dbResult, cache.KeyTimeout);
			return dbResult;
		}

		/// <summary>
		/// 更新时移除旧的缓存键
		/// </summary>
		public static void RemoveOldKeyWhenModify(params CacheKeyBase[] caches)
		{
			if (!caches.Any())
			{
				return;
			}

			foreach (var cache in caches)
			{
				if (_cacheQuery.KeyExists(cache.GetKey()))
				{
					_cacheQuery.KeyDelete(cache.GetKey());
				}
			}
		}

	}
}
