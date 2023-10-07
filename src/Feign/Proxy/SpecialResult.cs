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
    internal static class SpecialResults
    {
        private static readonly IDictionary<Type, object> s_handlers = new Dictionary<Type, object>();
        static SpecialResults()
        {
            s_handlers.Add(typeof(Task), SpecialResultHandler<Task>.FromResult(response => TaskEx.CompletedTask));
            s_handlers.Add(typeof(string), SpecialResultHandler<string>.FromTaskResult(response => response.Content.ReadAsStringAsync()));
            s_handlers.Add(typeof(Stream), SpecialResultHandler<Stream>.FromTaskResult(response => response.Content.ReadAsStreamAsync()));
            s_handlers.Add(typeof(byte[]), SpecialResultHandler<byte[]>.FromTaskResult(response => response.Content.ReadAsByteArrayAsync()));
            s_handlers.Add(typeof(HttpResponseMessage), SpecialResultHandler<HttpResponseMessage>.FromResult(response => response));
            s_handlers.Add(typeof(HttpContent), SpecialResultHandler<HttpContent>.FromResult(response => response.Content));
        }

        public static Task<SpecialResult<TResult>> GetSpecialResultAsync<TResult>(HttpResponseMessage responseMessage)
        {
            if (s_handlers.TryGetValue(typeof(TResult), out var handler))
            {
                return ((SpecialResultHandler<TResult>)handler).GetSpecialResultAsync(responseMessage);
            }
            SpecialResult<TResult> result = new SpecialResult<TResult>();
            return Task.FromResult(result);
        }

        public static bool IsSpecialResult(Type type)
        {
            return s_handlers.ContainsKey(type);
        }

    }

    internal class SpecialResultHandler<TResult>
    {
        public SpecialResultHandler(Func<HttpResponseMessage, Task<TResult>> asyncFunc)
        {
            _asyncFunc = asyncFunc;
        }

        public SpecialResultHandler(Func<HttpResponseMessage, TResult> func)
        {
            _func = func;
        }

        private readonly Func<HttpResponseMessage, Task<TResult>>? _asyncFunc;
        private readonly Func<HttpResponseMessage, TResult>? _func;

        public static SpecialResultHandler<TResult> FromResult(Func<HttpResponseMessage, TResult> func)
        {
            return new SpecialResultHandler<TResult>(func);
        }

        public static SpecialResultHandler<TResult> FromTaskResult(Func<HttpResponseMessage, Task<TResult>> asyncFunc)
        {
            return new SpecialResultHandler<TResult>(asyncFunc);
        }

        public Task<SpecialResult<TResult>> GetSpecialResultAsync(HttpResponseMessage responseMessage)
        {
            if (_asyncFunc != null)
            {
                return SpecialResult<TResult>.GetSpecialResultAsync(_asyncFunc(responseMessage));
            }
            return SpecialResult<TResult>.GetSpecialResultAsync(_func!(responseMessage));
        }
    }

    internal struct SpecialResult<TResult>
    {
        public bool IsSpecialResult { get; set; }
        public TResult Result { get; set; }
        public static async Task<SpecialResult<TResult>> GetSpecialResultAsync<TSource>(Task<TSource> task)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)(await task.ConfigureAwait(false))!;
            return specialResult;
        }
        public static Task<SpecialResult<TResult>> GetSpecialResultAsync<TSource>(TSource result)
        {
            SpecialResult<TResult> specialResult = new SpecialResult<TResult>()
            {
                IsSpecialResult = true
            };
            specialResult.Result = (TResult)(object)result!;
            return Task.FromResult(specialResult);
        }
    }
}
