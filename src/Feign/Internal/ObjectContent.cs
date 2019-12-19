//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//namespace Feign.Internal
//{
//    internal class ObjectContent : HttpContent
//    {

//        public ObjectContent(object value) : this(value, Encoding.UTF8)
//        {
//        }

//        public ObjectContent(object value, Encoding encoding) : this(value, encoding, null)
//        {
//        }

//        public ObjectContent(object value, Encoding encoding, MediaTypeHeaderValue mediaType)
//        {
//            Value = value;
//            Encoding = encoding;
//            SetHeaders(mediaType);
//        }


//        void SetHeaders(MediaTypeHeaderValue mediaType)
//        {
//            if (mediaType != null)
//            {
//                this.Headers.ContentType = (MediaTypeHeaderValue)((ICloneable)mediaType).Clone();
//            }
//            else
//            {
//                this.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
//                this.Headers.ContentType.CharSet = "utf-8";
//            }
//        }

//        public Object Value { get; }

//        public Encoding Encoding { get; }

//        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
//        {
//            if (this.Value == null)
//            {
//                return TaskEx.CompletedTask;
//            }
//            try
//            {
//                JsonHelper.Serialize(stream, Value, Value.GetType(), Encoding);
//                return TaskEx.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                return TaskEx.FromException(ex);
//            }
//        }

//        protected override bool TryComputeLength(out long length)
//        {
//            length = -1;
//            return false;
//        }


//    }


//    internal class ObjectContent<T> : ObjectContent
//    {
//        public ObjectContent(T value) : this(value, Encoding.UTF8)
//        {
//        }

//        public ObjectContent(T value, Encoding encoding) : this(value, encoding, null)
//        {
//        }

//        public ObjectContent(T value, Encoding encoding, MediaTypeHeaderValue mediaType) : base(value, encoding, mediaType)
//        {
//        }

//    }

//}
