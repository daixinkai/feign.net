using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    public interface IFeignClientTypeBuilder
    {        
        FeignClientTypeInfo Build(Type serviceType);
    }
}
