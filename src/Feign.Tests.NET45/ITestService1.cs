using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests.NET45
{
    public interface ITestService1
    {
        string Name { get; set; }
    }
    class TestService1 : ITestService1
    {
        public string Name { get; set; }
    }
}
