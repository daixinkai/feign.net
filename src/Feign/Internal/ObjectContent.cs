using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    internal class ObjectContent : HttpContent
    {

        public ObjectContent(object value) : this(value, Encoding.UTF8)
        {
        }

        public ObjectContent(object value, Encoding encoding) : this(value, encoding, null)
        {
        }

        public ObjectContent(object value, Encoding encoding, MediaTypeHeaderValue mediaType)
        {
            Value = value;
            Encoding = encoding;
            SetHeaders(mediaType);
        }


        void SetHeaders(MediaTypeHeaderValue mediaType)
        {
            if (mediaType != null)
            {
                this.Headers.ContentType = (MediaTypeHeaderValue)((ICloneable)mediaType).Clone();
            }
            else
            {
                this.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                this.Headers.ContentType.CharSet = "utf-8";
            }
        }

        public Object Value { get; }

        public Encoding Encoding { get; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (this.Value == null)
            {
#if NET45
                return Task.FromResult<object>(null);
#endif

#if NETSTANDARD
                return Task.CompletedTask;
#endif
            }
            try
            {
                using (JsonWriter jsonWriter = CreateJsonWriter(this.Value.GetType(), stream, this.Encoding))
                {
                    jsonWriter.CloseOutput = false;

                    JsonSerializer jsonSerializer = CreateJsonSerializer();
                    jsonSerializer.Serialize(jsonWriter, this.Value);
                    jsonWriter.Flush();
                }
#if NET45
                return Task.FromResult<object>(null);
#endif

#if NETSTANDARD
                return Task.CompletedTask;
#endif

            }
            catch (Exception ex)
            {
#if NET45
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                tcs.SetException(ex);
                return tcs.Task;
#endif

#if NETSTANDARD
                return Task.FromException(ex);
#endif

            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }


        /// <inheritdoc />
        JsonWriter CreateJsonWriter(Type type, Stream writeStream, Encoding effectiveEncoding)
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


        JsonSerializerSettings CreateDefaultSerializerSettings()
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
            JsonSerializer jsonSerializer = JsonSerializer.Create(CreateDefaultSerializerSettings());
            return jsonSerializer;
        }

    }


    internal class ObjectContent<T> : ObjectContent
    {
        public ObjectContent(T value) : this(value, Encoding.UTF8)
        {
        }

        public ObjectContent(T value, Encoding encoding) : this(value, encoding, null)
        {
        }

        public ObjectContent(T value, Encoding encoding, MediaTypeHeaderValue mediaType) : base(value, encoding, mediaType)
        {
        }

    }

}
