using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace CustomerManagementSystem.Utilities
{
    public static class CacheManager
    {
        private static MemoryCache _cache = MemoryCache.Default;
        private const string _captchaPrefix = "CAPTCHA_";
        public static void SetCacheKey(string key, string value, TimeSpan RandomDuration)
        {
            _cache.Set($"{_captchaPrefix}{key}", value, GetPolicy(RandomDuration));
        }

        public static string GetCacheKey(string key)
        {
            var cacheKey = _cache.Get($"{_captchaPrefix}{key}") as string;
            return cacheKey;
        }
        public static void RemoveCachekey(string key)
        {
            if (CacheKeyExists(key))
            {
                _cache.Remove($"{_captchaPrefix}{key}");
            }
        }
        public static bool CacheKeyExists(string key)
        {
            return GetCacheKey($"{key}") != null;
        }
        private static CacheItemPolicy GetPolicy(TimeSpan _cacheDuration)
        {
            return new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(_cacheDuration),
            };
        }
    }
}