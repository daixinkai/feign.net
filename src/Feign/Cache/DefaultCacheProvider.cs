using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Cache
{
    /// <summary>
    /// 默认缓存提供者,空实现
    /// </summary>
    public class DefaultCacheProvider : ICacheProvider
    {
        public T? Get<T>(string name)
        {
            return default;
        }

        public Task<T?> GetAsync<T>(string name)
        {
            return Task.FromResult(default(T));
        }

        public void Set<T>(string name, T value, TimeSpan? expirationTime)
        {
        }

        public Task SetAsync<T>(string name, T value, TimeSpan? expirationTime)
        {
            return TaskEx.CompletedTask;
        }
    }
}
