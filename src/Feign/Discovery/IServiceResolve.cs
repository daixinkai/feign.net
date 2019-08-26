using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    public interface IServiceResolve
    {
        Uri ResolveService(Uri uri, IList<IServiceInstance> services);
    }
}
