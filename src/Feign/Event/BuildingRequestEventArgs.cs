using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 表示BuildingRequest阶段的事件参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class BuildingRequestEventArgs<TService> : FeignClientEventArgs<TService>, IBuildingRequestEventArgs<TService>
    {
        internal BuildingRequestEventArgs(IFeignClient<TService> feignClient, string method, Uri requestUri, IDictionary<string, string> headers) : base(feignClient)
        {
            Method = method;
            RequestUri = requestUri;
            Headers = headers;
        }
        /// <summary>
        /// 获取http method
        /// </summary>
        public string Method { get; }
        /// <summary>
        /// 获取或设置请求路径
        /// </summary>
        public Uri RequestUri { get; set; }
        /// <summary>
        /// 获取请求头
        /// </summary>
        public IDictionary<string, string> Headers { get; }
    }

}
