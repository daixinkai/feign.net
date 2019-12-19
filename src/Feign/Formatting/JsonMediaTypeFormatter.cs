using Feign.Internal;
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
    /// 处理 application/json
    /// </summary>
    public class JsonMediaTypeFormatter : IMediaTypeFormatter
    {
        public JsonMediaTypeFormatter()
        {
            MediaType = Constants.MediaTypes.APPLICATION_JSON;
        }
        public string MediaType { get; }

        public TResult GetResult<TResult>(byte[] buffer, Encoding encoding)
        {
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return JsonHelper.DeserializeObject<TResult>(json);
        }

        public TResult GetResult<TResult>(Stream stream, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return GetResult<TResult>(buffer, encoding);
        }
    }
}
