#if NETSTANDARD2_1&&false
#define SYSTEM_TEXT_JSON
#else
#define NEWTONSOFT_JSON
#endif

#if NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif
#if SYSTEM_TEXT_JSON
using System.Text.Json;
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

#if SYSTEM_TEXT_JSON
        public static string SerializeObject(object value)
        {
            return JsonSerializer.Serialize(value, value.GetType());
        }

        public static TResult DeserializeObject<TResult>(string value)
        {
            return JsonSerializer.Deserialize<TResult>(value);
        }


        public static void Serialize(Stream stream, object value, Type type, Encoding encoding)
        {
            Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(stream);
            utf8JsonWriter.WriteStringValue(SerializeObject(value));
        }

#endif

#if NEWTONSOFT_JSON
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static TResult DeserializeObject<TResult>(string value)
        {
            return JsonConvert.DeserializeObject<TResult>(value);
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
            JsonSerializer jsonSerializer = JsonSerializer.Create(CreateDefaultSerializerSettings());
            return jsonSerializer;
        }
#endif



    }
}
