using Feign.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// Processing JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FeignClientHttpJsonRequestContent<T> : FeignClientHttpRequestContent
    {
        public FeignClientHttpJsonRequestContent(string name, T content)
        {
            Name = name;
            Content = content;
        }
        public string Name { get; }
        public T Content { get; }

        public override HttpContent? GetHttpContent(MediaTypeHeaderValue? contentType, IFeignOptions options)
        {
            Type type = typeof(T);
            if (type == typeof(byte[]) || typeof(Stream).IsAssignableFrom(type))
            {
                //throw new NotSupportedException($"Parameters of type {type.FullName} are not supported");
                return null;
            }
            if (contentType == null)
            {
                return new ObjectStringContent(Content, Encoding.UTF8, "application/json", options.JsonProvider);
            }
            ObjectStringContent content = new ObjectStringContent(Content, options.JsonProvider);
            content.Headers.ContentType = contentType;
            return content;
        }
    }
}
