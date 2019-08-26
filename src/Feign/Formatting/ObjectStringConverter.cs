using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    public sealed class ObjectStringConverter : IConverter<object, string>
    {
        public string Convert(object value)
        {
            return value?.ToString();
        }
    }
}
