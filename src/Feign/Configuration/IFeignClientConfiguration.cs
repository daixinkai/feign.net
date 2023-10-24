using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Configuration
{
    public interface IFeignClientConfiguration
    {
        void Configure(FeignClientConfigurationContext context);
    }

    public interface IFeignClientConfiguration<TService>
    {
        void Configure(FeignClientConfigurationContext<TService> context);
    }

}
