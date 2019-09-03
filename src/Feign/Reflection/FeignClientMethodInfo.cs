using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    public class FeignClientMethodInfo
    {
        public FeignClientMethodInfo()
        {
        }
        public FeignClientMethodInfo(string methodId, MethodInfo method)
        {
            MethodId = methodId;
            MethodMetadata = method;
        }

        public string MethodId { get; set; }

        public MethodInfo MethodMetadata { get; set; }

    }
}
