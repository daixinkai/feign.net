using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Cache
{
    /// <summary>
    /// 一个接口,表示缓存提供者
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T? Get<T>(string name);
        /// <summary>
        ///设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime">失效时间</param>
        void Set<T>(string name, T value, TimeSpan? expirationTime);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<T?> GetAsync<T>(string name);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime">失效时间</param>
        /// <returns></returns>
        Task SetAsync<T>(string name, T value, TimeSpan? expirationTime);

    }
}
