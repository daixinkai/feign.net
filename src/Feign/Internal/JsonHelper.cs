#if USE_SYSTEM_TEXT_JSON
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    static class JsonHelper
    {

        public static TResult DeserializeObject<TResult>(byte[] buffer, Encoding encoding)
        {
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject<TResult>(json);
        }

        public static object DeserializeObject(byte[] buffer, Type type, Encoding encoding)
        {
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject(json, type);
        }

        public static TResult DeserializeObject<TResult>(Stream stream, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return DeserializeObject<TResult>(buffer, encoding);
        }

        public static object DeserializeObject(Stream stream, Type type, Encoding encoding)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return DeserializeObject(buffer, type, encoding);
        }

#if USE_SYSTEM_TEXT_JSON

        internal readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static string SerializeObject(object value)
        {
            return JsonSerializer.Serialize(value, value.GetType(), _jsonSerializerOptions);
        }

        public static string SerializeObject(object value, Encoding encoding)
        {
            return JsonSerializer.Serialize(value, value.GetType(), _jsonSerializerOptions);
        }

        public static TResult DeserializeObject<TResult>(string value)
        {
            return JsonSerializer.Deserialize<TResult>(value, _jsonSerializerOptions);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return JsonSerializer.Deserialize(value, type, _jsonSerializerOptions);
        }


        public static void Serialize(Stream stream, object value, Type type, Encoding encoding)
        {
            using (Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(stream))
            {
                JsonSerializer.Serialize(utf8JsonWriter, value, _jsonSerializerOptions);
            }
        }
#else
        internal readonly static JsonSerializerSettings _jsonSerializerSettings = CreateDefaultSerializerSettings();

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public static string SerializeObject(object value, Encoding encoding)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public static TResult DeserializeObject<TResult>(string value)
        {
            return JsonConvert.DeserializeObject<TResult>(value, _jsonSerializerSettings);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _jsonSerializerSettings);
        }

        public static void Serialize(Stream stream, object value, Type type, Encoding encoding)
        {
            using (JsonWriter jsonWriter = CreateJsonWriter(type, stream, encoding))
            {
                jsonWriter.CloseOutput = false;

                JsonSerializer jsonSerializer = CreateJsonSerializer();
                jsonSerializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        static JsonWriter CreateJsonWriter(Type type, Stream writeStream, Encoding effectiveEncoding)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (writeStream == null)
            {
                throw new ArgumentNullException("writeStream");
            }

            if (effectiveEncoding == null)
            {
                throw new ArgumentNullException("effectiveEncoding");
            }

            JsonWriter jsonWriter = new JsonTextWriter(new StreamWriter(writeStream, effectiveEncoding));

            //if (Indent)
            //{
            //    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
            //}

            return jsonWriter;
        }
        static JsonSerializerSettings CreateDefaultSerializerSettings()
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
        static JsonSerializer CreateJsonSerializer()
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
            return jsonSerializer;
        }
#endif


    }
}
