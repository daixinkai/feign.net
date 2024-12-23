using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    internal class TestDateTimeOffsetToStringConverter : IConverter<DateTimeOffset, string>//, IStringConverter<DateTimeOffset>
    {
#if NET6_0_OR_GREATER
        public string Convert(DateTimeOffset value)
            => value.ToUnixTimeSeconds().ToString();
#else
        public string Convert(DateTimeOffset value)
            => value.ToString();
#endif
    }
}
