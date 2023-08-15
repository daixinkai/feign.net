using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign.Internal
{
    internal class ObjectStringContent : StringContent
    {
        public ObjectStringContent(object value, IJsonProvider jsonProvider) : this(value, Encoding.UTF8, jsonProvider) { }
        public ObjectStringContent(object value, Encoding encoding, IJsonProvider jsonProvider) : base(ToJson(value, encoding, jsonProvider), encoding, "application/json") { }
        public ObjectStringContent(object value, Encoding encoding, string mediaType, IJsonProvider jsonProvider) : base(ToJson(value, encoding, jsonProvider), encoding, mediaType) { }


        private static string ToJson(object value, Encoding encoding, IJsonProvider jsonProvider)
        {
            return jsonProvider.SerializeObject(value, encoding);
        }

    }
}
