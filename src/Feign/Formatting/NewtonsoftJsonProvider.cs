#if !USE_SYSTEM_TEXT_JSON
using Feign.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    internal class NewtonsoftJsonProvider : IJsonProvider
    {

        internal readonly JsonSerializerSettings _jsonSerializerSettings = CreateDefaultSerializerSettings();

        public TResult? DeserializeObject<TResult>(byte[] buffer, Encoding? encoding)
        {
            string json = EncodingEx.GetRequiredEncoding(encoding).GetString(buffer);
            return DeserializeObject<TResult>(json);
        }

        public object? DeserializeObject(byte[] buffer, Type type, Encoding? encoding)
        {
            string json = EncodingEx.GetRequiredEncoding(encoding).GetString(buffer);
            return DeserializeObject(json, type);
        }

        public TResult? DeserializeObject<TResult>(Stream stream, Encoding? encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return DeserializeObject<TResult>(buffer, encoding);
        }

        public object? DeserializeObject(Stream stream, Type type, Encoding? encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return DeserializeObject(buffer, type, encoding);
        }

        public string SerializeObject(object? value, Encoding? encoding)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public TResult? DeserializeObject<TResult>(string value)
        {
            return JsonConvert.DeserializeObject<TResult>(value, _jsonSerializerSettings);
        }

        public object? DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _jsonSerializerSettings);
        }

        private static JsonSerializerSettings CreateDefaultSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
#if !NETFX_CORE // DataContractResolver is not supported in portable library
                //ContractResolver = _defaultContractResolver,
#endif

                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Do not change this setting
                // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public Task<TResult?> DeserializeObjectAsync<TResult>(Stream stream, Encoding? encoding)
        {
            return Task.FromResult(DeserializeObject<TResult>(stream, encoding));
        }

        public Task<object?> DeserializeObjectAsync(Stream stream, Type type, Encoding? encoding)
        {
            return Task.FromResult(DeserializeObject(stream, type, encoding));
        }

        public void Configure(Action<JsonSerializerOptions> configure)
        {
            configure(_jsonSerializerSettings);
        }

    }
}

#endif