using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Cache
{
    public class DefaultCacheProvider : ICacheProvider
    {
        public T Get<T>(string name)
        {
            return default(T);
        }

        public Task<T> GetAsync<T>(string name)
        {
            return Task.FromResult(default(T));
        }

        public void Set<T>(string name, T value, TimeSpan? slidingExpiration)
        {
        }

        public Task SetAsync<T>(string name, T value, TimeSpan? slidingExpiration)
        {
            return TaskEx.CompletedTask;
        }
    }
}
