using Feign.Formatting;
using Feign.Internal;
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

        //internal static readonly MethodInfo HTTP_SEND_GENERIC_METHOD = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod).FirstOrDefault(o => o.Name == "Send");
        //internal static readonly MethodInfo HTTP_SEND_ASYNC_GENERIC_METHOD = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => o.IsGenericMethod).FirstOrDefault(o => o.Name == "SendAsync");

        //internal static readonly MethodInfo HTTP_SEND_METHOD = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod).FirstOrDefault(o => o.Name == "Send");
        //internal static readonly MethodInfo HTTP_SEND_ASYNC_METHOD = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !o.IsGenericMethod).FirstOrDefault(o => o.Name == "SendAsync");

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


        struct SpecialResult<T>
        {
            public bool IsSpecialResult { get; set; }
            public T Result { get; set; }
        }

        protected virtual async Task SendAsync(FeignClientHttpRequest request)
        {
            HttpResponseMessage response = await GetResponseMessageAsync(request)
#if  CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ;
            await GetResultAsync<string>(request, response)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ;
        }
        protected virtual async Task<TResult> SendAsync<TResult>(FeignClientHttpRequest request)
        {
            HttpResponseMessage response = await GetResponseMessageAsync(request)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ;
            return await GetResultAsync<TResult>(request, response)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ;
        }
        protected virtual void Send(FeignClientHttpRequest request)
        {
            HttpResponseMessage response = GetResponseMessage(request);
            GetResult<string>(request, response);
        }
        protected virtual TResult Send<TResult>(FeignClientHttpRequest request)
        {
            HttpResponseMessage response = GetResponseMessage(request);
            return GetResult<TResult>(request, response);
        }

        private HttpResponseMessage GetResponseMessage(FeignClientHttpRequest request)
        {
            try
            {
                return SendAsyncInternal(request).GetResult();
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
                ErrorRequestEventArgs<TService> errorArgs = new ErrorRequestEventArgs<TService>(this, ex);
                OnErrorRequest(errorArgs);
                if (errorArgs.ExceptionHandled)
                {
                    return null;
                }
                #endregion
                throw;
            }
        }

        private async Task<HttpResponseMessage> GetResponseMessageAsync(FeignClientHttpRequest request)
        {
            try
            {
                return await SendAsyncInternal(request)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                    ;
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
                ErrorRequestEventArgs<TService> errorArgs = new ErrorRequestEventArgs<TService>(this, ex);
                OnErrorRequest(errorArgs);
                if (errorArgs.ExceptionHandled)
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
        /// <param name="request"></param>
        /// <param name="responseMessage"></param>
        private void EnsureSuccess(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string content = responseMessage.Content.ReadAsStringAsync().GetResult();
                _logger?.LogError($"request on \"{responseMessage.RequestMessage.RequestUri.ToString()}\" status code : " + responseMessage.StatusCode.GetHashCode() + " content : " + content);
                throw new FeignHttpRequestException(this,
                    responseMessage.RequestMessage as FeignHttpRequestMessage,
                    new HttpRequestException($"Response status code does not indicate success: {responseMessage.StatusCode.GetHashCode()} ({responseMessage.ReasonPhrase}).\r\nContent : {content}"));
            }
        }
        /// <summary>
        /// 确保响应状态正确
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseMessage"></param>
        private async Task EnsureSuccessAsync(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync()
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                    ;
                _logger?.LogError($"request on \"{responseMessage.RequestMessage.RequestUri.ToString()}\" status code : " + responseMessage.StatusCode.GetHashCode() + " content : " + content);
                throw new FeignHttpRequestException(this,
                    responseMessage.RequestMessage as FeignHttpRequestMessage,
                    new HttpRequestException($"Response status code does not indicate success: {responseMessage.StatusCode.GetHashCode()} ({responseMessage.ReasonPhrase}).\r\nContent : {content}"));
            }
        }
        /// <summary>
        /// 获取响应结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        private TResult GetResult<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                return default(TResult);
            }
            #region ReceivingResponse
            ReceivingResponseEventArgs<TService, TResult> receivingResponseEventArgs = new ReceivingResponseEventArgs<TService, TResult>(this, responseMessage);
            OnReceivingResponse(receivingResponseEventArgs);
            //if (receivingResponseEventArgs.Result != null)
            if (receivingResponseEventArgs._isSetResult)
            {
                return receivingResponseEventArgs.GetResult<TResult>();
            }
            #endregion
            EnsureSuccess(request, responseMessage);

            var specialResult = GetSpecialResult<TResult>(responseMessage);
            if (specialResult.IsSpecialResult)
            {
                return specialResult.Result;
            }
            if (responseMessage.Content.Headers.ContentType == null && responseMessage.Content.Headers.ContentLength == 0)
            {
                return default(TResult);
            }
            IMediaTypeFormatter mediaTypeFormatter = _feignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                 responseMessage.RequestMessage as FeignHttpRequestMessage,
                 new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }
            return mediaTypeFormatter.GetResult<TResult>(responseMessage.Content.ReadAsStreamAsync().GetResult(), FeignClientUtils.GetEncoding(responseMessage.Content.Headers.ContentType));
        }
        /// <summary>
        /// 获取响应结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        private async Task<TResult> GetResultAsync<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                return default(TResult);
            }
            #region ReceivingResponse
            ReceivingResponseEventArgs<TService, TResult> receivingResponseEventArgs = new ReceivingResponseEventArgs<TService, TResult>(this, responseMessage);
            OnReceivingResponse(receivingResponseEventArgs);
            //if (receivingResponseEventArgs.Result != null)
            if (receivingResponseEventArgs._isSetResult)
            {
                return receivingResponseEventArgs.GetResult<TResult>();
            }
            #endregion
            await EnsureSuccessAsync(request, responseMessage)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ;

            var specialResult = await GetSpecialResultAsync<TResult>(responseMessage)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                 ;
            if (specialResult.IsSpecialResult)
            {
                return specialResult.Result;
            }

            if (responseMessage.Content.Headers.ContentType == null && responseMessage.Content.Headers.ContentLength == 0)
            {
                return default(TResult);
            }
            IMediaTypeFormatter mediaTypeFormatter = _feignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                     responseMessage.RequestMessage as FeignHttpRequestMessage,
                     new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }
            return mediaTypeFormatter.GetResult<TResult>(
                await responseMessage.Content.ReadAsStreamAsync()
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ,
                FeignClientUtils.GetEncoding(responseMessage.Content.Headers.ContentType)
                );
        }

        private SpecialResult<TResult> GetSpecialResult<TResult>(HttpResponseMessage responseMessage)
        {
            if (typeof(TResult) == typeof(Task))
            {
                return GetSpecialResult<Task, TResult>(TaskEx.CompletedTask);
            }
            else if (typeof(TResult) == typeof(string))
            {
                return GetSpecialResult<string, TResult>(responseMessage.Content.ReadAsStringAsync());
            }
            else if (typeof(TResult) == typeof(Stream))
            {
                return GetSpecialResult<Stream, TResult>(responseMessage.Content.ReadAsStreamAsync());
            }
            else if (typeof(TResult) == typeof(byte[]))
            {
                return GetSpecialResult<byte[], TResult>(responseMessage.Content.ReadAsByteArrayAsync());
            }
            else if (typeof(TResult) == typeof(HttpResponseMessage))
            {
                return GetSpecialResult<HttpResponseMessage, TResult>(responseMessage);
            }
            else if (typeof(TResult) == typeof(HttpContent))
            {
                return GetSpecialResult<HttpContent, TResult>(responseMessage.Content);
            }
            SpecialResult<TResult> result = new SpecialResult<TResult>();
            return result;
        }
        private Task<SpecialResult<TResult>> GetSpecialResultAsync<TResult>(HttpResponseMessage responseMessage)
        {
            if (typeof(TResult) == typeof(Task))
            {
                return GetSpecialResultAsync<Task, TResult>(TaskEx.CompletedTask);
            }
            else if (typeof(TResult) == typeof(string))
            {
                return GetSpecialResultAsync<string, TResult>(responseMessage.Content.ReadAsStringAsync());
            }
            else if (typeof(TResult) == typeof(Stream))
            {
                return GetSpecialResultAsync<Stream, TResult>(responseMessage.Content.ReadAsStreamAsync());
            }
            else if (typeof(TResult) == typeof(byte[]))
            {
                return GetSpecialResultAsync<byte[], TResult>(responseMessage.Content.ReadAsByteArrayAsync());
            }
            else if (typeof(TResult) == typeof(HttpResponseMessage))
            {
                return GetSpecialResultAsync<HttpResponseMessage, TResult>(responseMessage);
            }
            else if (typeof(TResult) == typeof(HttpContent))
            {
                return GetSpecialResultAsync<HttpContent, TResult>(responseMessage.Content);
            }
            SpecialResult<TResult> result = new SpecialResult<TResult>();
            return Task.FromResult(result);
        }

        private SpecialResult<TResult> GetSpecialResult<TSource, TResult>(Task<TSource> task)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)task.GetResult();
            return specialResult;
        }

        private SpecialResult<TResult> GetSpecialResult<TSource, TResult>(TSource result)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)result;
            return specialResult;
        }
        private async Task<SpecialResult<TResult>> GetSpecialResultAsync<TSource,TResult>(Task<TSource> task)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)await task
#if CONFIGUREAWAIT_FALSE               
           .ConfigureAwait(false)
#endif
                    ;
            return specialResult;
        }

        private Task<SpecialResult<TResult>> GetSpecialResultAsync<TSource, TResult>(TSource result)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)result;
            return Task.FromResult(specialResult);
        }


        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Task<HttpResponseMessage> SendAsyncInternal(FeignClientHttpRequest request)
        {
            HttpMethod httpMethod = GetHttpMethod(request.HttpMethod);
            HttpRequestMessage httpRequestMessage = CreateRequestMessage(request, httpMethod, CreateUri(BuildUri(request.Uri)));
            // if support content
            if (IsSupportContent(httpMethod))
            {
                HttpContent httpContent = request.GetHttpContent();
                if (httpContent != null)
                {
                    httpRequestMessage.Content = httpContent;
                }
            }
            return HttpClient.SendAsync(httpRequestMessage);
        }

        bool IsSupportContent(HttpMethod httpMethod)
        {
            return
                httpMethod == HttpMethod.Post
                || httpMethod == HttpMethod.Put
                || httpMethod == HttpMethod.Delete
                ;
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
            if (request.Headers != null && request.Headers.Length > 0)
            {
                foreach (var header in request.Headers)
                {
                    string[] values = header.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 2)
                    {
                        requestMessage.Headers.TryAddWithoutValidation(values[0], values[1]);
                    }
                }
            }
            return requestMessage;
        }

        private Uri CreateUri(string uri) =>
            string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

        private string BuildUri(string uri)
        {
            if (BaseUrl == "")
            {
                return uri;
            }
            if (uri.StartsWith("/"))
            {
                return BaseUrl + uri;
            }
            return BaseUrl + "/" + uri;
        }


    }
}
