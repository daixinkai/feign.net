#if USE_SYSTEM_TEXT_JSON
using Feign.Internal;
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
        internal readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<TResult?> DeserializeObjectAsync<TResult>(Stream stream, Encoding? encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return await JsonSerializer.DeserializeAsync<TResult>(stream, _jsonSerializerOptions)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            Memory<byte> buffer = new();
            await stream.ReadAsync(buffer)
#if USE_CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                ;
            string json = EncodingEx.GetRequiredEncoding(encoding).GetString(buffer.ToArray());
            return JsonSerializer.Deserialize<TResult>(json, _jsonSerializerOptions);
        }

        public async Task<object?> DeserializeObjectAsync(Stream stream, Type type, Encoding? encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return await JsonSerializer.DeserializeAsync(stream, type, _jsonSerializerOptions)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            Memory<byte> buffer = new();
            await stream.ReadAsync(buffer)
#if USE_CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
                 ;
            string json = EncodingEx.GetRequiredEncoding(encoding).GetString(buffer.ToArray());
            return JsonSerializer.Deserialize(json, type, _jsonSerializerOptions);
        }

        public string SerializeObject(object? value, Encoding? encoding)
        {
            return JsonSerializer.Serialize(value, _jsonSerializerOptions);
        }

        public void Configure(Action<JsonSerializerOptions> configure)
        {
            configure(_jsonSerializerOptions);
        }

    }
}

#endif