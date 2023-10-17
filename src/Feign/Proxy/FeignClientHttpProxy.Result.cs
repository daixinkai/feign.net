using Feign.Formatting;
using Feign.Internal;
using Feign.Pipeline.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    partial class FeignClientHttpProxy<TService>
    {
        /// <summary>
        /// Get result
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="responseContext"></param>
        /// <returns></returns>
        private async Task<TResult?> GetResultAsync<TResult>(ResponsePipelineContext<TService, TResult> responseContext)
        {
            if (responseContext.ResponseMessage == null)
            {
                return default;
            }

            #region ReceivingResponse
            await OnReceivingResponseAsync(responseContext).ConfigureAwait(false);
            if (responseContext._isSetResult)
            {
                await OnReceivedResponseAsync(responseContext).ConfigureAwait(false);
                return responseContext.GetResult();
            }
            #endregion

            var result = await GetResultInternalAsync(responseContext).ConfigureAwait(false);
            responseContext._result = result;
            await OnReceivedResponseAsync(responseContext).ConfigureAwait(false);
            return result;
        }

        private async Task<TResult?> GetResultInternalAsync<TResult>(ResponsePipelineContext<TService, TResult> responseContext)
        {
            var request = responseContext.Request;
            var responseMessage = responseContext.ResponseMessage;
            if (request.IsSpecialResult && typeof(TResult) == typeof(HttpResponseMessage))
            {
                responseContext.SkipReleaseResponse = true;
                return (TResult)(object)responseMessage;
            }
            await EnsureSuccessAsync(responseMessage).ConfigureAwait(false);
            if (request.IsSpecialResult)
            {
                var specialResult = await SpecialResults.GetSpecialResultAsync<TResult>(responseMessage).ConfigureAwait(false);
                if (specialResult.IsSpecialResult)
                {
                    return specialResult.Result;
                }
            }

            if (responseMessage.Content.Headers.ContentType == null && responseMessage.Content.Headers.ContentLength == 0)
            {
                return default;
            }
            IMediaTypeFormatter? mediaTypeFormatter = FeignOptions.MediaTypeFormatters.FindFormatter(responseMessage.Content.Headers.ContentType?.MediaType);
            if (mediaTypeFormatter == null)
            {
                throw new FeignHttpRequestException(this,
                     (FeignHttpRequestMessage)responseMessage.RequestMessage!,
                     new HttpRequestException($"Content type '{responseMessage.Content.Headers.ContentType?.ToString()}' not supported"));
            }

            using var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (stream.CanSeek)
            {
                return await GetResultAsyncInternal<TResult>(mediaTypeFormatter, stream, responseMessage.Content.Headers.ContentType, request.Method!.ResultType).ConfigureAwait(false);
            }
            using Stream seekStream = new MemoryStream();
            await stream.CopyToAsync(seekStream).ConfigureAwait(false);
            seekStream.Seek(0, SeekOrigin.Begin);
            return await GetResultAsyncInternal<TResult>(mediaTypeFormatter, seekStream, responseMessage.Content.Headers.ContentType, request.Method!.ResultType).ConfigureAwait(false);

        }

        private static Task<TResult?> GetResultAsyncInternal<TResult>(IMediaTypeFormatter mediaTypeFormatter, Stream stream, MediaTypeHeaderValue? mediaTypeHeaderValue, Type? resultType)
        {
            if (resultType != null)
            {
                return mediaTypeFormatter.GetResultAsync<TResult>(resultType, stream, FeignClientUtils.GetEncoding(mediaTypeHeaderValue));
            }
            return mediaTypeFormatter.GetResultAsync<TResult>(stream, FeignClientUtils.GetEncoding(mediaTypeHeaderValue));
        }


    }
}
