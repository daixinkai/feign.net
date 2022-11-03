using Feign.Cache;
using Feign.Discovery;
using Feign.Fallback;
using Feign.Internal;
using Feign.Logging;
using Feign.Pipeline;
using Feign.Pipeline.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    /// <summary>
    /// 支持服务降级的HttpProxy
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TFallback"></typeparam>
    public abstract class FallbackFeignClientHttpProxy<TService, TFallback> : FeignClientHttpProxy<TService>, IFallbackFeignClient<TService>
        where TService : class
        where TFallback : TService
    {

        public FallbackFeignClientHttpProxy(TFallback fallback, IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider = null, ILoggerFactory loggerFactory = null) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
            Fallback = fallback;
        }
        /// <summary>
        /// 获取降级服务对象
        /// </summary>
        public virtual TService Fallback { get; }

        protected override bool IsResponseTerminatedRequest => false;
        #region Send Request

        #region define

        internal static MethodInfo GetHttpSendGenericFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.Name == "Send").FirstOrDefault();
        }

        internal static MethodInfo GetHttpSendAsyncGenericFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.Name == "SendAsync").FirstOrDefault();
        }

        internal static MethodInfo GetHttpSendFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.Name == "Send").FirstOrDefault();
        }

        internal static MethodInfo GetHttpSendAsyncFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.Name == "SendAsync").FirstOrDefault();
        }

        #endregion

        protected virtual async Task SendAsync(FeignClientHttpRequest request, Func<Task> fallback)
        {
            try
            {
                await SendAsync(request).ConfigureAwait(false);
            }
            catch (TerminatedRequestException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (fallback == null)
                {
                    throw;
                }
                bool invokeFallbackRequestResult = await InvokeFallbackRequestPipeline(request, fallback, ex as ServiceResolveFailException).ConfigureAwait(false);
                if (invokeFallbackRequestResult)
                {
                    throw;
                }
                await fallback.Invoke().ConfigureAwait(false);
            }
        }
        protected virtual async Task<TResult> SendAsync<TResult>(FeignClientHttpRequest request, Func<Task<TResult>> fallback)
        {
            try
            {
                return await SendAsync<TResult>(request).ConfigureAwait(false);
            }
            catch (TerminatedRequestException)
            {
                return default(TResult);
            }
            catch (Exception ex)
            {
                if (fallback == null)
                {
                    throw;
                }
                bool invokeFallbackRequestResult = await InvokeFallbackRequestPipeline(request, fallback, ex as ServiceResolveFailException).ConfigureAwait(false);
                if (invokeFallbackRequestResult)
                {
                    throw;
                }
                return await fallback.Invoke().ConfigureAwait(false);
            }
        }
        protected virtual void Send(FeignClientHttpRequest request, Action fallback)
        {
            try
            {
                Send(request);
            }
            catch (TerminatedRequestException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback, ex as ServiceResolveFailException).GetResult())
                {
                    throw;
                }
                fallback.Invoke();
            }
        }
        protected virtual TResult Send<TResult>(FeignClientHttpRequest request, Func<TResult> fallback)
        {
            try
            {
                return Send<TResult>(request);
            }
            catch (TerminatedRequestException)
            {
                return default(TResult);
            }
            catch (Exception ex)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback, ex as ServiceResolveFailException).GetResult())
                {
                    throw;
                }
                return fallback.Invoke();
            }
        }
        #endregion

        protected internal virtual async Task OnFallbackRequest(IFallbackRequestPipelineContext<TService> context)
        {
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.FallbackRequestAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.FallbackRequestAsync(context).ConfigureAwait(false);
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.FallbackRequestAsync(context).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// 触发服务降级事件
        /// </summary>
        /// <param name="request"></param>
        /// <param name="delegate"></param>
        /// <param name="serviceResolveFailException"></param>
        /// <returns></returns>
        private async Task<bool> InvokeFallbackRequestPipeline(FeignClientHttpRequest request, Delegate @delegate, ServiceResolveFailException serviceResolveFailException)
        {
            IFallbackProxy fallbackProxy = @delegate.Target as IFallbackProxy;
            FallbackRequestPipelineContext<TService> context;
            if (fallbackProxy == null)
            {
                //可能因为method parameters length=0 , 故没有生成匿名调用类
                context = new FallbackRequestPipelineContext<TService>(this, request, Fallback, null, @delegate.Method);
            }
            else
            {
                context = new FallbackRequestPipelineContext<TService>(this, request, Fallback, fallbackProxy, null);
            }
            await OnFallbackRequest(context).ConfigureAwait(false);
            return context.IsTerminated;
        }

    }
}
