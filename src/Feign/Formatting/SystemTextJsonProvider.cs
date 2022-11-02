#if USE_SYSTEM_TEXT_JSON
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    public class SystemTextJsonProvider : IJsonProvider
    {
        internal readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TResult DeserializeObject<TResult>(byte[] buffer, Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return JsonSerializer.Deserialize<TResult>(buffer, _jsonSerializerOptions);
            }
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject<TResult>(json);
        }

        public object DeserializeObject(byte[] buffer, Type type, Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return JsonSerializer.Deserialize(buffer, type, _jsonSerializerOptions);
            }
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject(json, type);
        }

        public TResult DeserializeObject<TResult>(Stream stream, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return DeserializeObject<TResult>(buffer, encoding);
        }

        public object DeserializeObject(Stream stream, Type type, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return DeserializeObject(buffer, type, encoding);
        }

        public Task<TResult> DeserializeObjectAsync<TResult>(Stream stream, Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return JsonSerializer.DeserializeAsync<TResult>(stream, _jsonSerializerOptions).AsTask();
            }
            return DeserializeObjectAsyncInternal<TResult>(stream, encoding);
        }

        private async Task<TResult> DeserializeObjectAsyncInternal<TResult>(Stream stream, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                 ;
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject<TResult>(json);
        }

        public Task<object> DeserializeObjectAsync(Stream stream, Type type, Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return JsonSerializer.DeserializeAsync(stream, type, _jsonSerializerOptions).AsTask();
            }
            return DeserializeObjectAsyncInternal(stream, type, encoding);
        }

        private async Task<object> DeserializeObjectAsyncInternal(Stream stream, Type type, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length)
#if CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                 ;
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject(json, type);
        }

        public string SerializeObject(object value)
        {
            return JsonSerializer.Serialize(value, value.GetType(), _jsonSerializerOptions);
        }

        public string SerializeObject(object value, Encoding encoding)
        {
            return JsonSerializer.Serialize(value, value.GetType(), _jsonSerializerOptions);
        }

        public TResult DeserializeObject<TResult>(string value)
        {
            return JsonSerializer.Deserialize<TResult>(value, _jsonSerializerOptions);
        }

        public object DeserializeObject(string value, Type type)
        {
            return JsonSerializer.Deserialize(value, type, _jsonSerializerOptions);
        }


        public void Serialize(Stream stream, object value, Type type, Encoding encoding)
        {
            using (Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(stream))
            {
                JsonSerializer.Serialize(utf8JsonWriter, value, _jsonSerializerOptions);
            }
        }


    }
}

#endif