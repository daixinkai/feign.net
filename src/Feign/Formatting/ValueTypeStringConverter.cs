using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    public sealed class ValueTypeToStringConverter<T> : IConverter<T, string> where T : struct
    {
        public string Convert(T value)
        {
            return value.ToString();
        }
    }
}
