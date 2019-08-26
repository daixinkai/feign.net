using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign.Internal
{
    class ObjectStringContent : StringContent
    {
        public ObjectStringContent(object value) : this(value, Encoding.UTF8) { }
        public ObjectStringContent(object value, Encoding encoding) : base(ToJson(value), encoding, "application/json") { }
        public ObjectStringContent(object value, Encoding encoding, string mediaType) : base(ToJson(value), encoding, mediaType) { }


        static string ToJson(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

    }
}
