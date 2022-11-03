using Feign.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    class SpecialResults
    {
        public static SpecialResult<TResult> GetSpecialResult<TResult>(HttpResponseMessage responseMessage)
        {
            if (typeof(TResult) == typeof(Task))
            {
                return SpecialResult<TResult>.GetSpecialResult(TaskEx.CompletedTask);
            }
            else if (typeof(TResult) == typeof(string))
            {
                return SpecialResult<TResult>.GetSpecialResult(responseMessage.Content.ReadAsStringAsync());
            }
            else if (typeof(TResult) == typeof(Stream))
            {
                return SpecialResult<TResult>.GetSpecialResult(responseMessage.Content.ReadAsStreamAsync());
            }
            else if (typeof(TResult) == typeof(byte[]))
            {
                return SpecialResult<TResult>.GetSpecialResult(responseMessage.Content.ReadAsByteArrayAsync());
            }
            else if (typeof(TResult) == typeof(HttpResponseMessage))
            {
                return SpecialResult<TResult>.GetSpecialResult(responseMessage);
            }
            else if (typeof(TResult) == typeof(HttpContent))
            {
                return SpecialResult<TResult>.GetSpecialResult(responseMessage.Content);
            }
            SpecialResult<TResult> result = new SpecialResult<TResult>();
            return result;
        }
        public static Task<SpecialResult<TResult>> GetSpecialResultAsync<TResult>(HttpResponseMessage responseMessage)
        {
            if (typeof(TResult) == typeof(Task))
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(TaskEx.CompletedTask);
            }
            else if (typeof(TResult) == typeof(string))
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(responseMessage.Content.ReadAsStringAsync());
            }
            else if (typeof(TResult) == typeof(Stream))
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(responseMessage.Content.ReadAsStreamAsync());
            }
            else if (typeof(TResult) == typeof(byte[]))
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(responseMessage.Content.ReadAsByteArrayAsync());
            }
            else if (typeof(TResult) == typeof(HttpResponseMessage))
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(responseMessage);
            }
            else if (typeof(TResult) == typeof(HttpContent))
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(responseMessage.Content);
            }
            SpecialResult<TResult> result = new SpecialResult<TResult>();
            return Task.FromResult(result);
        }
    }

    struct SpecialResult<TResult>
    {
        public bool IsSpecialResult { get; set; }
        public TResult Result { get; set; }

        public static SpecialResult<TResult> GetSpecialResult<TSource>(Task<TSource> task)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)task.GetResult();
            return specialResult;
        }
        public static SpecialResult<TResult> GetSpecialResult<TSource>(TSource result)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)result;
            return specialResult;
        }
        public static async Task<SpecialResult<TResult>> GetSpecialResultAsync<TSource>(Task<TSource> task)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)await task.ConfigureAwait(false);
            return specialResult;
        }
        public static Task<SpecialResult<TResult>> GetSpecialResultAsync<TSource>(TSource result)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)result;
            return Task.FromResult(specialResult);
        }
    }
}
