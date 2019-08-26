using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    public class FeignClientTypeInfo
    {
        public FeignClientTypeInfo(Type serviceType)
        {
            ServiceType = serviceType;
        }
        public Type ServiceType { get; }
        public Type ParentType { get; set; }

        public Type BuildType { get; set; }

    }
}
