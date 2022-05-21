using Feign.Formatting;
using Feign.Internal;
using Feign.Pipeline.Internal;
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
        private async Task<TResult> GetResultAsync<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                return default;
            }
            #region ReceivingResponse
            var receivingResponseContext = await InvokeReceivingResponseAsync<TResult>(request, responseMessage)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                ;
            //if (receivingResponseContext.Result != null)
            if (receivingResponseContext._isSetResult)
            {
                await InvokeReceivedResponseAsync(receivingResponseContext)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
                return receivingResponseContext.GetResult();
            }
            #endregion

            var result = await GetResultInternalAsync<TResult>(request, responseMessage)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                ;
            receivingResponseContext.Result = result;
            await InvokeReceivedResponseAsync(receivingResponseContext)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                ;
            return result;
        }

        private async Task<ReceivingResponsePipelineContext<TService, TResult>> InvokeReceivingResponseAsync<TResult>(FeignClientHttpRequest request, HttpResponseMessage responseMessage)
        {
            var receivingResponseContext = new ReceivingResponsePipelineContext<TService, TResult>(this, request, responseMessage);
            await OnReceivingResponseAsync(receivingResponseContext)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                ;
            return receivingResponseContext;
        }

        private async Task InvokeReceivedResponseAsync<TResult>(ReceivingResponsePipelineContext<TService, TResult> context)
        {
            await OnReceivedResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                ;
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
