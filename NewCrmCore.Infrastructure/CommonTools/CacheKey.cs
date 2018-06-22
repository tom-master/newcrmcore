using System;

namespace NewCrmCore.Infrastructure.CommonTools
{
	public static class CacheKey
	{

		public class ConfigCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public ConfigCacheKey(Int32 identity)
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:Config:AccountId:{1}", CachePrefix, _identity);
			}
		}

		public class WallpaperCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public WallpaperCacheKey(Int32 identity)
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:Wallpaper:WallpaperId:{1}", CachePrefix, _identity);
			}
		}

		public class AccountCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public AccountCacheKey(Int32 identity)
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:Account:AccountId:{1}", CachePrefix, _identity);
			}
		}

		public class AccountRoleCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public AccountRoleCacheKey(Int32 identity)
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:Roles:AccountId:{1}", CachePrefix, _identity);
			}
		}

		public class PowersCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public PowersCacheKey(Int32 identity)
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:Powers", CachePrefix, _identity);
			}
		}

		public class DesktopCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public DesktopCacheKey(Int32 identity)
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:Desktop:AccountId:{1}", CachePrefix, _identity);
			}
		}

		public class AppTypeCacheKey : CacheKeyBase
		{
			private Int32 _identity;

			public AppTypeCacheKey(Int32 identity = default(Int32))
			{
				_identity = identity;
			}

			protected override string FormatKey()
			{
				return String.Format("{0}:AppTypes", CachePrefix, _identity);
			}
		}
	}
}
