using Feign.Request;
using Feign.Request.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Feign.Tests
{
    public class CustomHttpRequestTransform<T> : IHttpRequestTransform
    {

        public CustomHttpRequestTransform(T parameter)
        {
            _parameter = parameter;
        }

        private readonly T _parameter;

        public ValueTask ApplyAsync(FeignHttpRequestMessage request)
        {
            return default;
        }
    }


    public class CustomHttpRequestTransformAttribute : RequestParameterTransformBaseAttribute
    {
        protected override LocalBuilder EmitNewHttpRequestTransform(ILGenerator iLGenerator, LocalBuilder valueBuilder)
        {
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(IHttpRequestTransform));
            iLGenerator.Emit(OpCodes.Ldloc, valueBuilder);
            iLGenerator.Emit(OpCodes.Newobj, typeof(CustomHttpRequestTransform<>).MakeGenericType(valueBuilder.LocalType).GetConstructor(new Type[] { valueBuilder.LocalType })!);
            iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            return localBuilder;
        }
    }
}
