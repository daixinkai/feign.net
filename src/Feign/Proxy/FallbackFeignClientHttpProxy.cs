using Feign.Cache;
using Feign.Discovery;
using Feign.Fallback;
using Feign.Internal;
using Feign.Logging;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    public abstract class FallbackFeignClientHttpProxy<TService, TFallback> : FeignClientHttpProxy<TService>, IFallbackFeignClient<TService>
        where TService : class
        where TFallback : TService
    {

        public FallbackFeignClientHttpProxy(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory, TFallback fallback) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
            Fallback = fallback;
        }
        public virtual TService Fallback { get; }

        protected override bool IsResponseTerminatedRequest => false;

        #region Send Request

        #region define
        //internal static readonly MethodInfo HTTP_SEND_GENERIC_METHOD_FALLBACK = typeof(FallbackFeignClientHttpProxy<,>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.GetParameters().Length == 2).FirstOrDefault(o => o.Name == "Send");
        //internal static readonly MethodInfo HTTP_SEND_ASYNC_GENERIC_METHOD_FALLBACK = typeof(FallbackFeignClientHttpProxy<,>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.GetParameters().Length == 2).FirstOrDefault(o => o.Name == "SendAsync");

        //internal static readonly MethodInfo HTTP_SEND_METHOD_FALLBACK = typeof(FallbackFeignClientHttpProxy<,>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.GetParameters().Length == 2).FirstOrDefault(o => o.Name == "Send");
        //internal static readonly MethodInfo HTTP_SEND_ASYNC_METHOD_FALLBACK = typeof(FallbackFeignClientHttpProxy<,>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.GetParameters().Length == 2).FirstOrDefault(o => o.Name == "SendAsync");

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

        protected async Task SendAsync(FeignClientHttpRequest request, Func<Task> fallback)
        {
            try
            {
                await SendAsync(request);
            }
            catch (TerminatedRequestException)
            {
                return;
            }
            catch (Exception)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback))
                {
                    throw;
                }
                await fallback.Invoke();
            }
        }
        protected async Task<TResult> SendAsync<TResult>(FeignClientHttpRequest request, Func<Task<TResult>> fallback)
        {
            try
            {
                return await SendAsync<TResult>(request);
            }
            catch (TerminatedRequestException)
            {
                return default(TResult);
            }
            catch (Exception)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback))
                {
                    throw;
                }
                return await fallback.Invoke();
            }
        }
        protected void Send(FeignClientHttpRequest request, Action fallback)
        {
            try
            {
                Send(request);
            }
            catch (TerminatedRequestException)
            {
                return;
            }
            catch (Exception)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback))
                {
                    throw;
                }
                fallback.Invoke();
            }
        }
        protected TResult Send<TResult>(FeignClientHttpRequest request, Func<TResult> fallback)
        {
            try
            {
                return Send<TResult>(request);
            }
            catch (TerminatedRequestException)
            {
                return default(TResult);
            }
            catch (Exception)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback))
                {
                    throw;
                }
                return fallback.Invoke();
            }
        }
        #endregion

        protected internal virtual void OnFallbackRequest(FallbackRequestEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnFallbackRequest(this, e);
            _serviceIdFeignClientPipeline?.OnFallbackRequest(this, e);
            _globalFeignClientPipeline?.OnFallbackRequest(this, e);
        }

        bool InvokeFallbackRequestPipeline(FeignClientHttpRequest request, Delegate @delegate)
        {
            IFallbackProxy fallbackProxy = @delegate.Target as IFallbackProxy;
            FallbackRequestEventArgs<TService> eventArgs;
            if (fallbackProxy == null)
            {
                //可能因为method parameters length=0 , 故没有生成匿名调用类
                eventArgs = new FallbackRequestEventArgs<TService>(this, request, Fallback, null, @delegate.Method);
            }
            else
            {
                eventArgs = new FallbackRequestEventArgs<TService>(this, request, Fallback, fallbackProxy, null);
            }
            OnFallbackRequest(eventArgs);
            return eventArgs.IsTerminated;
        }

    }
}
