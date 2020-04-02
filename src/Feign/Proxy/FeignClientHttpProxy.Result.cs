using Feign.Formatting;
using Feign.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
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
            ReceivingResponseEventArgs<TService, TResult> receivingResponseEventArgs = new ReceivingResponseEventArgs<TService, TResult>(this, responseMessage);
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
            IMediaTypeFormatter mediaTypeFormatter = _feignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                 responseMessage.RequestMessage as FeignHttpRequestMessage,
                 new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }
            if (request.Method.ResultType!=null)
            {
                return (TResult)mediaTypeFormatter.GetResult(
                    request.Method.ResultType,
                    responseMessage.Content.ReadAsStreamAsync().GetResult(),
                    FeignClientUtils.GetEncoding(responseMessage.Content.Headers.ContentType)
                );
            }
            return mediaTypeFormatter.GetResult<TResult>(
                responseMessage.Content.ReadAsStreamAsync().GetResult(),
                FeignClientUtils.GetEncoding(responseMessage.Content.Headers.ContentType)
            );
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
            IMediaTypeFormatter mediaTypeFormatter = _feignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                     responseMessage.RequestMessage as FeignHttpRequestMessage,
                     new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }

            if (request.Method.ResultType != null)
            {
                return (TResult)mediaTypeFormatter.GetResult(
                    request.Method.ResultType,
                    await responseMessage.Content.ReadAsStreamAsync()
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                ,
                    FeignClientUtils.GetEncoding(responseMessage.Content.Headers.ContentType)
                    );
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


    }
}
