using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NewLibCore.Data.Redis.InternalHelper;
using NewLibCore.InternalExtension;

namespace NewCrmCore.Infrastructure.CommonTools
{

    public class CacheHelper
    {
        private static readonly ICacheQueryProvider _cacheQuery;

        static CacheHelper()
        {
            _cacheQuery = new DefaultRedisQueryProvider(0, Appsetting.Redis);
        }

        private CacheHelper() { }

        /// <summary>
        /// 获取或设置缓存
        /// </summary>
        public static async Task<TModel> GetOrSetCacheAsync<TModel>(CacheKeyBase cache, Func<Task<TModel>> func = null) where TModel : class
        {

            TModel cacheResult = null;
            try
            {
                if (typeof(TModel).IsComplexType())
                {
                    cacheResult = await Task.Run(() => _cacheQuery.StringGetAsync<TModel>(cache.GetKey()), cache.CancelToken);
                }
                else
                {
                    cacheResult = await Task.Run(() => _cacheQuery.StringGetAsync(cache.GetKey()), cache.CancelToken) as TModel;
                }
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
        public static async Task RemoveKeyWhenModify(params CacheKeyBase[] cacheKeys)
        {
            if (cacheKeys == null || !cacheKeys.Any())
            {
                return;
            }

            await Task.Run(() =>
            {
                foreach (var key in cacheKeys)
                {
                    _cacheQuery.KeyDelete(key.GetKey());
                }
            });
        }
    }
}
