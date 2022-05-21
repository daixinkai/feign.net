﻿using Feign.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// 表示BuildingRequest阶段的事件参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
#if NET5_0_OR_GREATER
    internal record BuildingRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, IBuildingRequestPipelineContext<TService>
#else
    internal class BuildingRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, IBuildingRequestPipelineContext<TService>
#endif
    {
        internal BuildingRequestPipelineContext(IFeignClient<TService> feignClient, string method, Uri requestUri, IDictionary<string, string> headers, FeignClientHttpRequest request) : base(feignClient)
        {
            Method = method;
            RequestUri = requestUri;
            Headers = headers;
            Request = request;
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
        /// <summary>
        /// 获取请求
        /// </summary>
        public FeignClientHttpRequest Request { get; }
    }

}
