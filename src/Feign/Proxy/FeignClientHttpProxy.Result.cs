using Feign.Formatting;
using Feign.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    partial class FeignClientHttpProxy<TService>
    {
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
            ReceivingResponseEventArgs<TService, TResult> receivingResponseEventArgs = InvokeReceivingResponseEvent<TResult>(request, responseMessage);
            //if (receivingResponseEventArgs.Result != null)
            if (receivingResponseEventArgs._isSetResult)
            {
                return receivingResponseEventArgs.GetResult();
            }
            #endregion

            return GetResultInternal<TResult>(request, responseMessage);

        }

        /// <summary>
        /// 获取响应结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        private Task<TResult> GetResultAsync<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                return Task.FromResult(default(TResult));
            }
            #region ReceivingResponse
            ReceivingResponseEventArgs<TService, TResult> receivingResponseEventArgs = InvokeReceivingResponseEvent<TResult>(request, responseMessage);
            //if (receivingResponseEventArgs.Result != null)
            if (receivingResponseEventArgs._isSetResult)
            {
                return Task.FromResult(receivingResponseEventArgs.GetResult());
            }
            #endregion

            return GetResultInternalAsync<TResult>(request, responseMessage);
        }


        private ReceivingResponseEventArgs<TService, TResult> InvokeReceivingResponseEvent<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            ReceivingResponseEventArgs<TService, TResult> receivingResponseEventArgs = new ReceivingResponseEventArgs<TService, TResult>(this, request, responseMessage);
            OnReceivingResponse(receivingResponseEventArgs);
            return receivingResponseEventArgs;
        }


        private TResult GetResultInternal<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            EnsureSuccess(request, responseMessage);
            var specialResult = SpecialResults.GetSpecialResult<TResult>(responseMessage);
            if (specialResult.IsSpecialResult)
            {
                return specialResult.Result;
            }
            if (responseMessage.Content.Headers.ContentType == null && responseMessage.Content.Headers.ContentLength == 0)
            {
                return default(TResult);
            }
            IMediaTypeFormatter mediaTypeFormatter = FeignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                 responseMessage.RequestMessage as FeignHttpRequestMessage,
                 new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }
            using (var stream = responseMessage.Content.ReadAsStreamAsync().GetResult())
            {
                if (stream.CanSeek)
                {
                    return GetResultInternal<TResult>(mediaTypeFormatter, stream, responseMessage.Content.Headers.ContentType, request.Method.ResultType);
                }
                using (Stream seekStream = new MemoryStream())
                {
                    stream.CopyTo(seekStream);
                    seekStream.Position = 0;
                    return GetResultInternal<TResult>(mediaTypeFormatter, seekStream, responseMessage.Content.Headers.ContentType, request.Method.ResultType);
                }
            }
        }

        private async Task<TResult> GetResultInternalAsync<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            await EnsureSuccessAsync(request, responseMessage)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ;

            var specialResult = await SpecialResults.GetSpecialResultAsync<TResult>(responseMessage)
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
            IMediaTypeFormatter mediaTypeFormatter = FeignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                     responseMessage.RequestMessage as FeignHttpRequestMessage,
                     new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }

            using (var stream = await responseMessage.Content.ReadAsStreamAsync()
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
           )
            {
                if (stream.CanSeek)
                {
                    return await GetResultAsyncInternal<TResult>(mediaTypeFormatter, stream, responseMessage.Content.Headers.ContentType, request.Method.ResultType)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                        ;
                }
                using (Stream seekStream = new MemoryStream())
                {
                    await stream.CopyToAsync(seekStream)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                        ;
                    seekStream.Position = 0;
                    return await GetResultAsyncInternal<TResult>(mediaTypeFormatter, seekStream, responseMessage.Content.Headers.ContentType, request.Method.ResultType)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
           ;
                }
            }

        }


        private TResult GetResultInternal<TResult>(IMediaTypeFormatter mediaTypeFormatter, Stream stream, System.Net.Http.Headers.MediaTypeHeaderValue mediaTypeHeaderValue, Type resultType)
        {
            if (resultType != null)
            {
                return (TResult)mediaTypeFormatter.GetResult(resultType, stream, FeignClientUtils.GetEncoding(mediaTypeHeaderValue));
            }
            return mediaTypeFormatter.GetResult<TResult>(stream, FeignClientUtils.GetEncoding(mediaTypeHeaderValue));
        }

        private async Task<TResult> GetResultAsyncInternal<TResult>(IMediaTypeFormatter mediaTypeFormatter, Stream stream, System.Net.Http.Headers.MediaTypeHeaderValue mediaTypeHeaderValue, Type resultType)
        {
            if (resultType != null)
            {
                return (TResult)await mediaTypeFormatter.GetResultAsync(resultType, stream, FeignClientUtils.GetEncoding(mediaTypeHeaderValue))
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
           ;
            }
            return await mediaTypeFormatter.GetResultAsync<TResult>(stream, FeignClientUtils.GetEncoding(mediaTypeHeaderValue))
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
           ;
        }

    }
}
