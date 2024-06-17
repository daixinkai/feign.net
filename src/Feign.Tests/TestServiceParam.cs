using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.Tests
{
    public abstract class TestServiceParamBase
    {
        public abstract string ServiceId { get; }
    }
    public class TestServiceParam : TestServiceParamBase
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int? State { get; set; }

        public string[] Ids { get; set; }

        public TestServiceParam SubParam { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public override string ServiceId => null;
    }
}
