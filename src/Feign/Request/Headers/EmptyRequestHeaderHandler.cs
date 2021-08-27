using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request.Headers
{
    public sealed class EmptyRequestHeaderHandler : IRequestHeaderHandler
    {
        public void SetHeader(HttpRequestMessage httpRequestMessage)
        {
        }
    }
}
