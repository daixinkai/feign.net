using Feign.Formatting;
using Feign.Internal;
using Feign.Pipeline.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    partial class FeignClientHttpProxy<TService>
    {
        #region Define

        internal static MethodInfo GetHttpSendGenericMethod(Type serviceType)
        {
            return typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.Name == "Send").FirstOrDefault();
        }

        internal static MethodInfo GetHttpSendAsyncGenericMethod(Type serviceType)
        {
            return typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod && o.Name == "SendAsync").FirstOrDefault();
        }

        internal static MethodInfo GetHttpSendMethod(Type serviceType)
        {
            return typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.Name == "Send").FirstOrDefault();
        }

        internal static MethodInfo GetHttpSendAsyncMethod(Type serviceType)
        {
            return typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod && o.Name == "SendAsync").FirstOrDefault();
        }

        #endregion

        protected virtual async Task SendAsync(FeignClientHttpRequest request)
        {
            HttpResponseMessage response = await GetResponseMessageAsync(request).ConfigureAwait(false);
            if (response == null)
            {
                return;
            }
            using (response)
            {
                await EnsureSuccessAsync(response).ConfigureAwait(false);
            }

        }
        protected virtual async Task<TResult> SendAsync<TResult>(FeignClientHttpRequest request)
        {
            HttpResponseMessage response = await GetResponseMessageAsync(request).ConfigureAwait(false);
            if (response == null)
            {
                return default;
            }
            var responseContext = new ResponsePipelineContext<TService, TResult>(this, request, response);
            try
            {
                return await GetResultAsync(responseContext).ConfigureAwait(false);
            }
            finally
            {
                if (!responseContext.SkipReleaseResponse)
                {
                    response.Dispose();
                }
            }
        }

        protected virtual void Send(FeignClientHttpRequest request)
            => SendAsync(request).WaitEx();
        protected virtual TResult Send<TResult>(FeignClientHttpRequest request)
            => SendAsync<TResult>(request).GetResult();

        private async Task<HttpResponseMessage> GetResponseMessageAsync(FeignClientHttpRequest request)
        {
            try
            {
                return await SendAsyncInternal(request).ConfigureAwait(false);
            }
            catch (TerminatedRequestException)
            {
                if (IsResponseTerminatedRequest)
                {
                    return null;
                }
                throw;
            }
            catch (ServiceResolveFailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                #region ErrorRequest
                ErrorRequestPipelineContext<TService> errorContext = new ErrorRequestPipelineContext<TService>(this, ex);
                await OnErrorRequestAsync(errorContext).ConfigureAwait(false);
                if (errorContext.ExceptionHandled)
                {
                    return null;
                }
                #endregion
                throw;
            }
        }

        /// <summary>
        /// 确保响应状态正确
        /// </summary>
        /// <param name="responseMessage"></param>
        private async Task EnsureSuccessAsync(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger?.LogError($"request on \"{responseMessage.RequestMessage.RequestUri}\" status code : " + responseMessage.StatusCode.GetHashCode() + " content : " + content);
                throw new FeignHttpRequestException(this,
                    responseMessage.RequestMessage as FeignHttpRequestMessage,
                    new HttpRequestException($"Response status code does not indicate success: {responseMessage.StatusCode.GetHashCode()} ({responseMessage.ReasonPhrase}).\r\nContent : {content}"));
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendAsyncInternal(FeignClientHttpRequest request)
        {
            HttpMethod httpMethod = GetHttpMethod(request.HttpMethod);
            HttpRequestMessage httpRequestMessage = CreateRequestMessage(request, httpMethod, CreateUri(request));
            using (httpRequestMessage)
            {
                // if support content
                if (IsSupportContent(httpMethod))
                {
                    HttpContent httpContent = request.GetHttpContent(FeignOptions);
                    if (httpContent != null)
                    {
                        httpRequestMessage.Content = httpContent;
                    }
                }
                return await HttpClient.SendAsync(httpRequestMessage, request.CompletionOption).ConfigureAwait(false);
            }
        }

        private bool IsSupportContent(HttpMethod httpMethod)
        {
            return httpMethod.IsSupportContent();
        }

        private HttpMethod GetHttpMethod(string method)
        {
            HttpMethod httpMethod;
            switch (method.ToUpper())
            {
                case "GET":
                    httpMethod = HttpMethod.Get;
                    break;
                case "POST":
                    httpMethod = HttpMethod.Post;
                    break;
                case "PUT":
                    httpMethod = HttpMethod.Put;
                    break;
                case "DELETE":
                    httpMethod = HttpMethod.Delete;
                    break;
                case "HEAD":
                    httpMethod = HttpMethod.Head;
                    break;
                case "OPTIONS":
                    httpMethod = HttpMethod.Options;
                    break;
                case "TRACE":
                    httpMethod = HttpMethod.Trace;
                    break;
                default:
                    httpMethod = new HttpMethod(method);
                    break;
            }
            return httpMethod;
        }

        private HttpRequestMessage CreateRequestMessage(FeignClientHttpRequest request, HttpMethod method, Uri uri)
        {
            FeignHttpRequestMessage requestMessage = new FeignHttpRequestMessage(request, method, uri);
            if (!string.IsNullOrWhiteSpace(request.Accept))
            {
                requestMessage.Headers.Accept.ParseAdd(request.Accept);
            }
            var headers = DefaultHeaders;
            if (headers != null || request.Headers != null)
            {
                if (headers == null)
                {
                    headers = request.Headers;
                }
                else if (request.Headers != null)
                {
                    headers = headers.Concat(request.Headers).ToArray();
                }
                foreach (var header in headers)
                {
                    string[] values = header.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 2)
                    {
                        requestMessage.Headers.TryAddWithoutValidation(values[0], values[1]);
                    }
                }
            }
            if (request.RequestHeaderHandlers != null && request.RequestHeaderHandlers.Count > 0)
            {
                foreach (var handler in request.RequestHeaderHandlers)
                {
                    handler.SetHeader(requestMessage);
                }
            }
            return requestMessage;
        }

        private Uri CreateUri(FeignClientHttpRequest request)
        {
            string uri = BuildUri(request);
            return string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);
        }


        private string BuildUri(FeignClientHttpRequest request)
        {
            string uri = request.Uri;
            string baseUrl = UriKind == UriKind.Absolute ? Origin : BaseUrl;
            if (baseUrl == "")
            {
                return uri;
            }
            if (uri.StartsWith("/"))
            {
                return UriKind == UriKind.RelativeOrAbsolute ? $"{Origin}{uri}" : $"{baseUrl}{uri}";
            }
            return uri.StartsWith("?") ? $"{baseUrl}{uri}" : $"{baseUrl}/{uri}";
        }


    }
}
