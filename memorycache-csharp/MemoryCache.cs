using System;
using System.Runtime.Caching;
using CacheImpl = System.Runtime.Caching.MemoryCache;

namespace App.Common.Caching
{
    public class MemoryCache : IMemoryCache
    {
        private static readonly CacheItemPolicy DefaultCachePolicy = new CacheItemPolicy();

        private readonly CacheImpl _cache;

        public MemoryCache() : this(CacheImpl.Default)
        {
        }

        public MemoryCache(CacheImpl cache)
        {
            _cache = cache;
        }

        public T AddOrGetExisting<T>(
            string key,
            Func<T> valueFactory,
            CacheItemPolicy cacheItemPolicy = null)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue =
                _cache.AddOrGetExisting(
                    key,
                    newValue,
                    cacheItemPolicy ?? DefaultCachePolicy) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                _cache.Remove(key);
                throw;
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}