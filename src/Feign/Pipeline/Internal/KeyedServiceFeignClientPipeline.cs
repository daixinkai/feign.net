using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal class KeyedServiceFeignClientPipeline<TService> : ServiceFeignClientPipeline<TService>
    {
        public KeyedServiceFeignClientPipeline(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
