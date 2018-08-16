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
                return String.Format("Config:AccountId:{0}", _identity);
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
                return String.Format("Wallpaper:WallpaperId:{0}", _identity);
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
                return String.Format("Account:AccountId:{0}", _identity);
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
                return String.Format("Roles:AccountId:{0}", _identity);
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
                return String.Format("Powers");
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
                return String.Format("Desktop:AccountId:{0}", _identity);
            }
        }

        public class AppTypeCacheKey : CacheKeyBase
        {
            private Int32 _identity;

            public AppTypeCacheKey(Int32 identity = 0)
            {
                _identity = identity;
            }

            protected override string FormatKey()
            {
                return String.Format("AppTypes:{0}", _identity);
            }
        }

        public class GlobalUniqueTokenCacheKey : CacheKeyBase
        {
            public String _identity;

            public GlobalUniqueTokenCacheKey(String identity)
            {
                _identity = identity;
            }

            protected override string FormatKey()
            {
                return String.Format("GlobalUniqueToken:{0}", _identity);
            }
        }
    }
}
