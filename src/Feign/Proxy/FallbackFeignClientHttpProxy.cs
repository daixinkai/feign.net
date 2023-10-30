using Feign.Fallback;
using Feign.Internal;
using Feign.Pipeline;
using Feign.Pipeline.Internal;
using Feign.Request;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    /// <summary>
    /// HttpProxy with service fallback support
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TFallback"></typeparam>
    public abstract class FallbackFeignClientHttpProxy<TService, TFallback> : FeignClientHttpProxy<TService>, IFallbackFeignClient<TService>
        where TService : class
        where TFallback : TService
    {

        public FallbackFeignClientHttpProxy(
            TFallback fallback,
            FeignClientHttpProxyOptions options
            )
            : base(options)
        {
            Fallback = fallback;
        }
        /// <summary>
        /// Gets the Fallback object
        /// </summary>
        public virtual TService Fallback { get; }

        protected override bool IsResponseTerminatedRequest => false;
        #region Send Request

        #region define

        internal static MethodInfo GetHttpSendGenericFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.Name == "Send").FirstOrDefault()!;
        }

        internal static MethodInfo GetHttpSendAsyncGenericFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.Name == "SendAsync").FirstOrDefault()!;
        }

        internal static MethodInfo GetHttpSendFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.Name == "Send").FirstOrDefault()!;
        }

        internal static MethodInfo GetHttpSendAsyncFallbackMethod(Type serviceType, Type fallbackType)
        {
            return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, fallbackType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.Name == "SendAsync").FirstOrDefault()!;
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
                bool invokeFallbackRequestResult = await InvokeFallbackRequestPipeline(request, fallback, ex).ConfigureAwait(false);
                if (invokeFallbackRequestResult)
                {
                    throw;
                }
                await fallback.Invoke().ConfigureAwait(false);
            }
        }
        protected virtual async Task<TResult?> SendAsync<TResult>(FeignClientHttpRequest request, Func<Task<TResult>> fallback)
        {
            try
            {
                return await SendAsync<TResult>(request).ConfigureAwait(false);
            }
            catch (TerminatedRequestException)
            {
                return default;
            }
            catch (Exception ex)
            {
                if (fallback == null)
                {
                    throw;
                }
                bool invokeFallbackRequestResult = await InvokeFallbackRequestPipeline(request, fallback, ex).ConfigureAwait(false);
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
                if (InvokeFallbackRequestPipeline(request, fallback, ex).GetResult())
                {
                    throw;
                }
                fallback.Invoke();
            }
        }
        protected virtual TResult? Send<TResult>(FeignClientHttpRequest request, Func<TResult> fallback)
        {
            try
            {
                return Send<TResult>(request);
            }
            catch (TerminatedRequestException)
            {
                return default;
            }
            catch (Exception ex)
            {
                if (fallback == null)
                {
                    throw;
                }
                if (InvokeFallbackRequestPipeline(request, fallback, ex).GetResult())
                {
                    throw;
                }
                return fallback.Invoke();
            }
        }
        #endregion

        protected internal virtual async Task OnFallbackRequest(IFallbackRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.FallbackRequestAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.FallbackRequestAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.FallbackRequestAsync(context).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// invoke fallback
        /// </summary>
        /// <param name="request"></param>
        /// <param name="delegate"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private async Task<bool> InvokeFallbackRequestPipeline(FeignClientHttpRequest request, Delegate @delegate, Exception exception)
        {
            IFallbackProxy? fallbackProxy = @delegate.Target as IFallbackProxy;
            FallbackRequestPipelineContext<TService> context;
            if (fallbackProxy == null)
            {
                //可能因为method parameters length=0 , 故没有生成匿名调用类
                context = new FallbackRequestPipelineContext<TService>(this, request, Fallback, null, @delegate.Method, exception);
            }
            else
            {
                context = new FallbackRequestPipelineContext<TService>(this, request, Fallback, fallbackProxy, null, exception);
            }
            await OnFallbackRequest(context).ConfigureAwait(false);
            return context.IsTerminated;
        }

    }
}
