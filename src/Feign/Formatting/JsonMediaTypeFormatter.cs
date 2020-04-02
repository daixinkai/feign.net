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
#if USE_SYSTEM_TEXT_JSON
        public JsonSerializerOptions JsonSerializerOptions { get; }
#else
        public JsonSerializerSettings JsonSerializerSettings { get; }
#endif

        public JsonMediaTypeFormatter() : this(Constants.MediaTypes.APPLICATION_JSON)
        {
        }

        public JsonMediaTypeFormatter(string mediaType)
        {
#if USE_SYSTEM_TEXT_JSON
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
#else
            JsonSerializerSettings = new JsonSerializerSettings();
#endif
            MediaType = mediaType;
        }
        public string MediaType { get; }

        public TResult GetResult<TResult>(byte[] buffer, Encoding encoding)
        {
            return JsonHelper.DeserializeObject<TResult>(buffer, encoding);
        }

        public TResult GetResult<TResult>(Stream stream, Encoding encoding)
        {
            return JsonHelper.DeserializeObject<TResult>(stream, encoding);
        }

        public object GetResult(Type type, Stream stream, Encoding encoding)
        {
            return JsonHelper.DeserializeObject(stream, type, encoding);
        }
    }
}
