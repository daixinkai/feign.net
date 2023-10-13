using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Cache
{
    /// <summary>
    /// An interface representing the cache provider
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Get cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T? Get<T>(string name);
        /// <summary>
        /// Set cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime">expiration time</param>
        void Set<T>(string name, T value, TimeSpan? expirationTime);
        /// <summary>
        /// Get cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<T?> GetAsync<T>(string name);
        /// <summary>
        /// Set cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime">expiration time</param>
        /// <returns></returns>
        Task SetAsync<T>(string name, T value, TimeSpan? expirationTime);

    }
}
