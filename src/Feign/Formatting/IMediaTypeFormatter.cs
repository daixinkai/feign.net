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
        TResult GetResult<TResult>(byte[] buffer, Encoding encoding);
    }
}
