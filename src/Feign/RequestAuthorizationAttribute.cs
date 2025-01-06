using Feign.Request.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Sets Authorization to the request headers
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RequestAuthorizationAttribute : RequestParameterTransformBaseAttribute
    {
        public RequestAuthorizationAttribute()
        {
        }

        public RequestAuthorizationAttribute(string scheme)
        {
            Scheme = scheme;
        }

        public string? Scheme { get; }


        public static KeyValuePair<string, string> GetHeader(string scheme, string value)
        {
            if (!string.IsNullOrWhiteSpace(scheme))
            {
                return new KeyValuePair<string, string>("Authorization", scheme + " " + value);
            }
            return new KeyValuePair<string, string>("Authorization", value);
        }

        protected internal override LocalBuilder? EmitNewHttpRequestTransform(ILGenerator iLGenerator, LocalBuilder valueBuilder)
        {
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(IHttpRequestTransform));
            var method = typeof(RequestAuthorizationAttribute).GetMethod("GetHeader", BindingFlags.Public | BindingFlags.Static);
            //iLGenerator.Emit(OpCodes.Pop);
            //iLGenerator.Emit(OpCodes.Ldnull);
            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                iLGenerator.Emit(OpCodes.Ldstr, Scheme);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            iLGenerator.Emit(OpCodes.Ldloc, valueBuilder);
            if (valueBuilder.LocalType != typeof(string))
            {
                iLGenerator.Emit(OpCodes.Call, valueBuilder.LocalType.GetConvertToStringValueMethod());
            }
            iLGenerator.Emit(OpCodes.Call, method!);
            iLGenerator.Emit(OpCodes.Newobj, typeof(HttpRequestHeaderTransform).GetConstructor(new Type[] { typeof(KeyValuePair<string, string>) })!);
            iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            return localBuilder;
        }

    }
}
