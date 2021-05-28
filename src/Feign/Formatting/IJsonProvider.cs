using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    public interface IJsonProvider
    {
        TResult DeserializeObject<TResult>(byte[] buffer, Encoding encoding);
        object DeserializeObject(byte[] buffer, Type type, Encoding encoding);
        TResult DeserializeObject<TResult>(Stream stream, Encoding encoding);
        object DeserializeObject(Stream stream, Type type, Encoding encoding);
        string SerializeObject(object value);
        string SerializeObject(object value, Encoding encoding);
        TResult DeserializeObject<TResult>(string value);
        object DeserializeObject(string value, Type type);
        void Serialize(Stream stream, object value, Type type, Encoding encoding);
    }
}
