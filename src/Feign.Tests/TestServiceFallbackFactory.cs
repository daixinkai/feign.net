using Feign.Fallback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.Tests
{
    public class TestServiceFallbackFactory : IFallbackFactory<ITestService>
    {
        public ITestService GetFallback()
        {
            return new TestServiceFallback();
        }

        public void ReleaseFallback(ITestService fallback)
        {
           
        }
    }
}
