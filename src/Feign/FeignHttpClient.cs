using Feign.Internal;
using Feign.Request;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
    /// </summary>
    public abstract class FeignHttpClient : IDisposable
    {

        private static readonly ConcurrentDictionary<string, HttpHandlerType> s_handlerMap = new();

        public FeignHttpClient(HttpMessageHandler handler)
        {
            _handler = new FeignDelegatingHandler(handler);
            Channel = new HttpClient(_handler);
        }

        private readonly DelegatingHandler _handler;

        public HttpClient Channel { get; }

        private bool disposedValue;

        /// <summary>
        /// Gets or sets the inner handler which processes the HTTP response messages.
        /// </summary>
        public HttpMessageHandler Handler
        {
            get => _handler.InnerHandler!;
            set => _handler.InnerHandler = value;
        }

        internal HttpHandlerType? HttpHandler => Handler as HttpHandlerType;

        public abstract Task<HttpResponseMessage> SendAsync(FeignHttpRequestMessage request, HttpCompletionOption completionOption);

        public static HttpHandlerType GetDefaultHandler(string serviceId, FeignClientLifetime lifetime, FeignOptions options)
        {
            if (lifetime == FeignClientLifetime.Singleton)
            {
                if (s_handlerMap.TryGetValue(serviceId, out var handler))
                {
                    return handler;
                }
                handler = CreateDefaultHandler(serviceId, options);
                s_handlerMap.TryAdd(serviceId, handler);
                return handler;
            }
            return CreateDefaultHandler(serviceId, options);
        }

        private static HttpHandlerType CreateDefaultHandler(string serviceId, FeignOptions options)
        {
            var handler = new HttpHandlerType()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };
#if NETCOREAPP2_1_OR_GREATER
            if (options.Request.PooledConnectionLifetime.HasValue)
            {
                handler.PooledConnectionLifetime = options.Request.PooledConnectionLifetime.Value;
            }
#endif
            if (options.Request.AutomaticDecompression.HasValue)
            {
                handler.AutomaticDecompression = options.Request.AutomaticDecompression.Value;
            }
            if (options.Request.UseCookies.HasValue)
            {
                handler.UseCookies = options.Request.UseCookies.Value;
            }
            return handler;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Channel.Dispose();
                }
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~FeignHttpClient()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
