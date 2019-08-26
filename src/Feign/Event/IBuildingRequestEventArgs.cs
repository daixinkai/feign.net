using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IBuildingRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        string Method { get; }
        Uri RequestUri { get; set; }
        IDictionary<string, string> Headers { get; }
    }
}
