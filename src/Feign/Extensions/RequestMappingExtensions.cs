using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class RequestMappingExtensions
    {
        public static bool IsHttpMethod(this IRequestMapping requestMapping, HttpMethod httpMethod)
        {
            if (requestMapping == null)
            {
                return false;
            }
            return httpMethod.Method.Equals(requestMapping.GetMethod(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
