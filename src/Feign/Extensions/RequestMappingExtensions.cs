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
        public static bool IsHttpMethod(this IRequestMapping requestMapping, HttpMethod httpMethod)
        {
            if (requestMapping == null)
            {
                return false;
            }
            return string.Equals(httpMethod.Method, requestMapping.GetMethod(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHttpMethod(this IRequestMapping requestMapping, string httpMethod)
        {
            if (requestMapping == null)
            {
                return false;
            }
            return string.Equals(httpMethod, requestMapping.GetMethod(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSupportRequestContent(this IRequestMapping requestMapping)
        {
            return
                requestMapping.IsHttpMethod(HttpMethod.Post) ||
                requestMapping.IsHttpMethod(HttpMethod.Put) ||
                requestMapping.IsHttpMethod(HttpMethod.Delete) ||
                requestMapping.IsHttpMethod("PATCH")
                ;
        }

        public static bool IsSupportContent(this HttpMethod httpMethod)
        {
            return
                httpMethod == HttpMethod.Post
                || httpMethod == HttpMethod.Put
                || httpMethod == HttpMethod.Delete
                //Patch also supports
                || string.Equals(httpMethod.Method, "PATCH", StringComparison.OrdinalIgnoreCase)
                 ;
        }

    }
}
