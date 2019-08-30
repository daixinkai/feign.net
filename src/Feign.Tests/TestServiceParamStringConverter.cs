using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Tests
{
    class TestServiceParamStringConverter : IConverter<TestServiceParam, string>
    {
        public string Convert(TestServiceParam value)
        {
            return value.ToString();
        }
    }
}
