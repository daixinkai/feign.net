using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal class KeyedServiceIdFeignClientPipeline : ServiceIdFeignClientPipeline
    {
        public KeyedServiceIdFeignClientPipeline(string key, string serviceId) : base(serviceId)
        {
            Key = key;
        }
        public string Key { get; }
    }
}
