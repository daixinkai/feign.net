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

        public JsonMediaTypeFormatter(IFeignOptions options) : this(Constants.MediaTypes.APPLICATION_JSON, options)
        {
        }

        public JsonMediaTypeFormatter(string mediaType, IFeignOptions options)
        {
            MediaType = mediaType;
            _options = options;
        }
        public string MediaType { get; }

        private IFeignOptions _options;

        public Task<TResult?> GetResultAsync<TResult>(Stream stream, Encoding? encoding)
        {
            return _options.JsonProvider.DeserializeObjectAsync<TResult>(stream, encoding);
        }

        public Task<object?> GetResultAsync(Type type, Stream stream, Encoding? encoding)
        {
            return _options.JsonProvider.DeserializeObjectAsync(stream, type, encoding);
        }
    }
}
