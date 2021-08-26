using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    /// <summary>
    /// 媒体类型处理
    /// </summary>
    public interface IMediaTypeFormatter
    {
        string MediaType { get; }

        TResult GetResult<TResult>(Stream stream, Encoding encoding);
        object GetResult(Type type, Stream stream, Encoding encoding);

        Task<TResult> GetResultAsync<TResult>(Stream stream, Encoding encoding);
        Task<object> GetResultAsync(Type type, Stream stream, Encoding encoding);

        //Task<TResult> ReadAsync<TResult>(Stream stream, Encoding encoding);
        //Task WriteAsync<TSource>(TSource source, Stream stream, Encoding encoding);
    }
}
