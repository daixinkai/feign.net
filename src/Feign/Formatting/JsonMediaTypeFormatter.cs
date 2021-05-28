#if USE_SYSTEM_TEXT_JSON
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

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

        public TResult GetResult<TResult>(byte[] buffer, Encoding encoding)
        {
            return _options.JsonProvider.DeserializeObject<TResult>(buffer, encoding);
        }

        public TResult GetResult<TResult>(Stream stream, Encoding encoding)
        {
            return _options.JsonProvider.DeserializeObject<TResult>(stream, encoding);
        }

        public object GetResult(Type type, Stream stream, Encoding encoding)
        {
            return _options.JsonProvider.DeserializeObject(stream, type, encoding);
        }
    }
}
