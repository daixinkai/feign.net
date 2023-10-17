using Feign.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// Processing Form
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FeignClientHttpFormRequestContent<T> : FeignClientHttpRequestContent
    {
        public FeignClientHttpFormRequestContent(string name, T? content)
        {
            Name = name;
            Content = content;
        }
        public string Name { get; private set; }
        public T? Content { get; }

        public override HttpContent? GetHttpContent(MediaTypeHeaderValue? contentType, IFeignOptions options)
        {
            Type type = typeof(T);
            if (!type.IsValueType && Content == null)
            {
                return null;
            }
            if (type == typeof(byte[]))
            {
                //throw new NotSupportedException();
                return null;
            }
            if (typeof(Stream).IsAssignableFrom(type))
            {
                //throw new NotSupportedException();
                return null;
            }

            if (Type.GetTypeCode(type) != TypeCode.Object)
            {
                return new StringContent(Content?.ToString() ?? "");
            }

            IList<KeyValuePair<string, string?>> nameValueCollection =
                Content is IHttpRequestForm httpRequestForm ?
                (httpRequestForm.GetRequestForm()?.ToList() ?? ArrayEx.EmptyList<KeyValuePair<string, string?>>()) :
                FeignClientUtils.GetObjectStringParameters(Name, Content, options.Converters, options.PropertyNamingPolicy).ToList();
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(nameValueCollection);
            return formUrlEncodedContent;
        }

    }
}
