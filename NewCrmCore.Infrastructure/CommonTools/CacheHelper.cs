using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewCrmCore.Dto;
using NewLibCore;
using NewLibCore.Data.Redis.InternalHelper;

namespace NewCrmCore.Infrastructure.CommonTools
{

	public class CacheHelper
	{
		private static readonly ICacheQueryProvider _cacheQuery;

		static CacheHelper()
		{
			_cacheQuery = new DefaultRedisQueryProvider(0, Appsetting.Redis);
		}

		public static async Task<TModel> GetOrSetCache<TModel>(CacheKeyBase cache, Func<Task<TModel>> func = null) where TModel : class
		{

			TModel cacheResult = null;
			try
			{
				cacheResult = await Task.Run(() => _cacheQuery.StringGetAsync<TModel>(cache.GetKey()), cache.CancelToken);
			}
			catch (OperationCanceledException)
			{

			}

			if (cacheResult != null)
			{
				return cacheResult;
			}

			if (func != null)
			{
				var dbResult = await func();
				_cacheQuery.StringSet(cache.GetKey(), dbResult, cache.KeyTimeout);
				return dbResult;
			}
			return default(TModel);
		}

		/// <summary>
		/// 更新时移除旧的缓存键
		/// </summary>
		public static async Task RemoveKeyWhenModify(params CacheKeyBase[] caches)
		{
			if (!caches.Any())
			{
				return;
			}

			await Task.Run(() =>
			{
				foreach (var cache in caches)
				{
					_cacheQuery.KeyDelete(cache.GetKey());
				}
			});
		}
	}
}
