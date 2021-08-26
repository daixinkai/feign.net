using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Feign.Formatting
{
    /// <summary>
    /// 支持 xml
    /// </summary>
    public class XmlMediaTypeFormatter : IMediaTypeFormatter
    {
        public XmlMediaTypeFormatter() : this(Constants.MediaTypes.APPLICATION_XML)
        {
        }
        public XmlMediaTypeFormatter(string mediaType)
        {
            MediaType = mediaType;
        }
        public string MediaType { get; }

        public HttpContent GetHttpContent(object content, MediaTypeHeaderValue contentType)
        {
            if (content == null)
            {
                return null;
            }

            if (content is Stream)
            {
                return new StreamContent((Stream)content);
            }

            if (content is byte[])
            {
                return new ByteArrayContent((byte[])content);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding("utf8"));
                XmlSerializer xz = new XmlSerializer(content.GetType());
                xz.Serialize(sw, content);
                return new StreamContent(sw.BaseStream);
            }

        }

        public TResult GetResult<TResult>(Stream stream, Encoding encoding)
        {
            return (TResult)GetResult(typeof(TResult), stream, encoding);
        }

        public object GetResult(Type type, Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding))
            {
                XmlSerializer xz = new XmlSerializer(type);
                return xz.Deserialize(sr);
            }
        }

        public Task<TResult> GetResultAsync<TResult>(Stream stream, Encoding encoding)
        {
            return Task.FromResult(GetResult<TResult>(stream, encoding));
        }

        public Task<object> GetResultAsync(Type type, Stream stream, Encoding encoding)
        {
            return Task.FromResult(GetResult(type, stream, encoding));
        }
    }
}
