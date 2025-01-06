using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request.Transforms
{
    public interface IHttpRequestTransform
    {
        ValueTask ApplyAsync(FeignHttpRequestMessage request);
    }
}
