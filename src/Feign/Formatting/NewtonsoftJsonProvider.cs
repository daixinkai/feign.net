#if !USE_SYSTEM_TEXT_JSON
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

        public TResult DeserializeObject<TResult>(byte[] buffer, Encoding encoding)
        {
            string json = (encoding ?? Encoding.Default).GetString(buffer);
            return DeserializeObject<TResult>(json);
        }

        public object DeserializeObject(byte[] buffer, Type type, Encoding encoding)
        {
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

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public string SerializeObject(object value, Encoding encoding)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public TResult DeserializeObject<TResult>(string value)
        {
            return JsonConvert.DeserializeObject<TResult>(value, _jsonSerializerSettings);
        }

        public object DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _jsonSerializerSettings);
        }

        public void Serialize(Stream stream, object value, Type type, Encoding encoding)
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
        JsonSerializer CreateJsonSerializer()
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
            return jsonSerializer;
        }

        public Task<TResult> DeserializeObjectAsync<TResult>(Stream stream, Encoding encoding)
        {
            return Task.FromResult(DeserializeObject<TResult>(stream, encoding));
        }

        public Task<object> DeserializeObjectAsync(Stream stream, Type type, Encoding encoding)
        {
            return Task.FromResult(DeserializeObject(stream, type, encoding));
        }
    }
}

#endif