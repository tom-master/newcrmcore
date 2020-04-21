using System;

namespace NewCrmCore.Infrastructure.CommonTools
{
    public static class CacheKey
    {

        public class TokenCacheKey : CacheKeyBase
        {
            private String _identity;
            public TokenCacheKey(String identity)
            {
                _identity = identity;
            }
            protected override string FormatKey()
            {
                return String.Format("Token:{0}", _identity);
            }
        }

        public class ConfigCacheKey : CacheKeyBase
        {
            private Int32 _identity;

            public ConfigCacheKey(Int32 identity)
            {
                _identity = identity;
            }

            protected override string FormatKey()
            {
                return String.Format("Config:UserId:{0}", _identity);
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

        public class UserCacheKey : CacheKeyBase
        {
            private Int32 _identity;

            public UserCacheKey(Int32 identity)
            {
                _identity = identity;
            }

            protected override string FormatKey()
            {
                return String.Format("User:UserId:{0}", _identity);
            }
        }

        public class UserRoleCacheKey : CacheKeyBase
        {
            private Int32 _identity;

            public UserRoleCacheKey(Int32 identity)
            {
                _identity = identity;
            }

            protected override string FormatKey()
            {
                return String.Format("Roles:UserId:{0}", _identity);
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
                return String.Format("Desktop:UserId:{0}", _identity);
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

        public class SignalRConnectionCacheKey : CacheKeyBase
        {
            public String _identity;

            public SignalRConnectionCacheKey(String identity)
            {
                _identity = identity;
            }

            public override TimeSpan? KeyTimeout
            {
                get { return null; }
            }

            protected override string FormatKey()
            {
                return String.Format("SignalRConnection:{0}", _identity);
            }
        }
    }
}
