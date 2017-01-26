using System;
using System.Runtime.Caching;

namespace App.Common.Caching
{
    public interface IMemoryCache
    {
        T AddOrGetExisting<T>(
            string key,
            Func<T> valueFactory,
            CacheItemPolicy cacheItemPolicy = null);

        void Remove(string key);
    }
}