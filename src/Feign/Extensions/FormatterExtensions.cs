using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    internal static class FormatterExtensions
    {
        public static async Task<TResult?> GetResultAsync<TResult>(this IMediaTypeFormatter mediaTypeFormatter, Type type, Stream stream, Encoding? encoding)
        {
            var result = await mediaTypeFormatter.GetResultAsync(type, stream, encoding)
#if USE_CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                 ;
            if (result == null)
            {
                return default;
            }
            return (TResult)result;
        }
    }
}
