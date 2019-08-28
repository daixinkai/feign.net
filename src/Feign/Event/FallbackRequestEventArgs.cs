using Feign.Fallback;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 表示服务降级时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class FallbackRequestEventArgs<TService> : FeignClientEventArgs<TService>, IFallbackRequestEventArgs<TService>
    {
        internal FallbackRequestEventArgs(IFeignClient<TService> feignClient, FeignClientHttpRequest request, TService fallback, IFallbackProxy fallbackProxy, MethodInfo method) : base(feignClient)
        {
            Request = request;
            Fallback = fallback;
            FallbackProxy = fallbackProxy;
            _method = method;
        }
        /// <summary>
        /// 获取请求对象
        /// </summary>
        public FeignClientHttpRequest Request { get; }
        /// <summary>
        /// 获取降级代理对象
        /// </summary>
        public IFallbackProxy FallbackProxy { get; }
        /// <summary>
        /// 获取降级服务对象
        /// </summary>
        public TService Fallback { get; }
        MethodInfo _method;
        /// <summary>
        /// 获取降级的服务方法
        /// </summary>
        public MethodInfo Method
        {
            get
            {
                if (_method == null)
                {
                    _method = Fallback.GetType().GetMethod(FallbackProxy.MethodName, FallbackProxy.GetParameterTypes());
                }
                return _method;
            }
        }
        /// <summary>
        /// 获取请求的参数描述
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetParameters()
        {
            return FallbackProxy?.GetParameters() ?? new Dictionary<string, object>();
        }
        /// <summary>
        /// 获取请求的参数类型
        /// </summary>
        /// <returns></returns>
        public Type[] GetParameterTypes()
        {
            return FallbackProxy?.GetParameterTypes() ?? Type.EmptyTypes;
        }
        /// <summary>
        /// 获取一个值,指示是否终止降级
        /// </summary>
        public bool IsTerminated => _isTerminated;

        bool _isTerminated;
        /// <summary>
        /// 终止降级
        /// </summary>
        public void Terminate()
        {
            _isTerminated = true;
        }

    }
}
