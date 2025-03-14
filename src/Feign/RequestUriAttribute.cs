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
    /// Convert parameters into request uri
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RequestUriAttribute : RequestParameterTransformBaseAttribute
    {
        protected internal override LocalBuilder? EmitNewHttpRequestTransform(ILGenerator iLGenerator, LocalBuilder valueBuilder)
        {
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(IHttpRequestTransform));
            iLGenerator.Emit(OpCodes.Ldloc, valueBuilder);
            if (valueBuilder.LocalType != typeof(string))
            {
                iLGenerator.Emit(OpCodes.Call, valueBuilder.LocalType.GetConvertToStringValueMethod());
            }
            iLGenerator.Emit(OpCodes.Newobj, typeof(HttpRequestUriTransform).GetConstructor(new Type[] { typeof(string) })!);
            iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            return localBuilder;
        }

    }
}
