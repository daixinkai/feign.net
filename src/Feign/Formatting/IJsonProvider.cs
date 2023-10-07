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
        string SerializeObject(object? value, Encoding? encoding);
        //TResult DeserializeObject<TResult>(Stream stream, Encoding? encoding);
        //object DeserializeObject(Stream stream, Type type, Encoding? encoding);
        Task<TResult?> DeserializeObjectAsync<TResult>(Stream stream, Encoding? encoding);
        Task<object?> DeserializeObjectAsync(Stream stream, Type type, Encoding? encoding);
    }
}
