using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// 默认转换 object to string
    /// </summary>
    public sealed class ObjectStringConverter : IConverter<object, string>
    {
        public string Convert(object value)
        {
            return value?.ToString();
        }
    }

    internal class ClassToStringConverter<T> : IConverter<T, string> where T : class
    {
        public string Convert(T value)
        {
            return value?.ToString();
        }
    }

    internal class StructToStringConverter<T> : IConverter<T, string> where T : struct
    {
        public string Convert(T value)
        {
            return value.ToString();
        }
    }

}
