using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Cache
{
    public interface ICacheProvider
    {
        T Get<T>(string name);
        void Set<T>(string name, T value, TimeSpan? slidingExpiration);

        Task<T> GetAsync<T>(string name);
        Task SetAsync<T>(string name, T value, TimeSpan? slidingExpiration);

    }
}
