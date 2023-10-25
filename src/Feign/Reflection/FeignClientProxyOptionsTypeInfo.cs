using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    public class FeignClientProxyOptionsTypeInfo
    {
        public FeignClientProxyOptionsTypeInfo(Type type, Type configurationType)
        {
            Type = type;
            ConfigurationType = configurationType;
        }
        public Type Type { get; }
        public Type ConfigurationType { get; }
    }
}
