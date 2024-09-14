using Feign.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    internal class TestConfiguration : IFeignClientConfiguration<ITestControllerService>, IFeignClientConfiguration
    {
        public void Configure(FeignClientConfigurationContext<ITestControllerService> context)
        {
            context.Pipeline.Authorization(feignClient =>
            {                
                return null;
            });
        }

        public void Configure(FeignClientConfigurationContext context)
        {
            context.Pipeline.Authorization(feignClient =>
            {
                return null;
            });
        }
    }
}
