using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Configuration
{
    public class FeignClientServiceConfiguration<TService>
    {
        public FeignClientServiceConfiguration()
        {

        }
        public IFeignClientConfiguration? Configuration { get; set; }
        public IFeignClientConfiguration<TService>? ServiceConfiguration { get; }
    }
}
