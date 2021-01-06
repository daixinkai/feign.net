using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Feign.Discovery;
using Microsoft.Extensions.Caching.Distributed;

namespace Feign.Cache
{
    class JsonCacheProvider : ICacheProvider
    {
        public JsonCacheProvider(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        IDistributedCache _distributedCache;


        public T Get<T>(string name)
        {
            var json = _distributedCache?.GetString(name);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return DeserializeFromCache<T>(json);
            }
            return default(T);
        }

        public async Task<T> GetAsync<T>(string name)
        {
            var json = await _distributedCache?.GetStringAsync(name);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return DeserializeFromCache<T>(json);
            }
            return default(T);
        }

        public void Set<T>(string name, T value, TimeSpan? expirationTime)
        {
            _distributedCache?.SetString(name, SerializeForCache(value), new DistributedCacheEntryOptions
            {
                //SlidingExpiration = expirationTime
                AbsoluteExpirationRelativeToNow = expirationTime
            });
        }

        public async Task SetAsync<T>(string name, T value, TimeSpan? expirationTime)
        {
            await _distributedCache?.SetStringAsync(name, SerializeForCache(value), new DistributedCacheEntryOptions
            {
                //SlidingExpiration = expirationTime
                AbsoluteExpirationRelativeToNow = expirationTime
            });
        }


        private static string SerializeForCache(object data)
        {
#if NETSTANDARD2_0
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
#else
            return System.Text.Json.JsonSerializer.Serialize(data);
#endif
        }

        private static T DeserializeFromCache<T>(string json)
        {
            try
            {
#if NETSTANDARD2_0
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
#else
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
#endif
            }
            catch
            {
                return default;
            }
        }



    }
}
