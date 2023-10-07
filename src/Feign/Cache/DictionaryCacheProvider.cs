using Feign.Cache;
using Feign.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Cache
{
    public class DictionaryCacheProvider : ICacheProvider
    {
        private class CacheEntry
        {
            public string Key { get; set; } = null!;
            public object Value { get; set; } = null!;
            public long? ExpirationTime { get; set; }
            public bool IsExpiration()
            {
                return ExpirationTime.HasValue && ExpirationTime.Value < GetCurrentTimeMillis();
            }
        }

        private readonly ConcurrentDictionary<string, CacheEntry> _map = new ConcurrentDictionary<string, CacheEntry>();

        public T? Get<T>(string name)
        {
            if (!_map.TryGetValue(name, out var cacheEntry))
            {
                return default;
            }
            return cacheEntry.IsExpiration() ? default : (T)cacheEntry.Value;
        }

        public Task<T?> GetAsync<T>(string name)
        {
            return Task.FromResult(Get<T>(name));
        }

        public void Set<T>(string name, T value, TimeSpan? expirationTime)
        {
            CacheEntry cacheEntry = new CacheEntry
            {
                Key = name,
                Value = value!,
                ExpirationTime = expirationTime.HasValue ? GetCurrentTimeMillis(expirationTime.Value) : null
            };
            _map.AddOrUpdate(name, cacheEntry, (key, oldValue) => cacheEntry);
        }

        public Task SetAsync<T>(string name, T value, TimeSpan? expirationTime)
        {
            Set(name, value, expirationTime);
            return TaskEx.CompletedTask;
        }

        private static long GetCurrentTimeMillis()
        {
#if NET45
            TimeSpan ts = DateTimeOffset.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
#else
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#endif
        }

        private static long GetCurrentTimeMillis(TimeSpan timeSpan)
        {
#if NET45
            TimeSpan ts = DateTimeOffset.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            ts.Add(timeSpan);
            return Convert.ToInt64(ts.TotalMilliseconds);
#else
            return DateTimeOffset.UtcNow.Add(timeSpan).ToUnixTimeMilliseconds();
#endif
        }

    }
}
