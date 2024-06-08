using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    internal static class RequestMappingExtensions
    {

        public static bool IsHttpMethod(this IRequestMapping requestMapping, string httpMethod)
        {
            if (requestMapping == null)
            {
                return false;
            }
            return string.Equals(httpMethod, requestMapping.GetMethod(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHttpMethod(this IRequestMapping requestMapping, HttpMethod httpMethod)
            => IsHttpMethod(requestMapping, httpMethod.Method);

        public static bool IsSupportRequestContent(this IRequestMapping requestMapping)
            => IsSupportContent(requestMapping.GetMethod());

        public static bool IsSupportContent(this HttpMethod httpMethod)
            => IsSupportContent(httpMethod.Method);

        private static bool IsSupportContent(string httpMethod)
        {
            return
                string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase)
                || string.Equals(httpMethod, "PUT", StringComparison.OrdinalIgnoreCase)
                || string.Equals(httpMethod, "DELETE", StringComparison.OrdinalIgnoreCase)
                //Patch also supports
                || string.Equals(httpMethod, "PATCH", StringComparison.OrdinalIgnoreCase)
                 ;
        }

    }
}
