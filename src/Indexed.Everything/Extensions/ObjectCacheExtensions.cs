using System;
using System.Runtime.Caching;

namespace Indexed.Everything.Extensions
{
    internal static class ObjectCacheExtensions
    {
        private const int CacheDurationInMinutes = 10;

        private static readonly CacheItemPolicy CacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(CacheDurationInMinutes) };

        public static T GetOrAdd<T>(this ObjectCache cache, string key, Func<T> get, CacheItemPolicy cacheItemPolicy = null) where T : class
        {
            if (cache.Contains(key))
            {
                return cache.Get(key) as T;
            }

            T value = get();
            cache.Add(key, value, cacheItemPolicy ?? CacheItemPolicy);
            return value;
        }
    }
}
