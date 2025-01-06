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
    /// Convert parameters into request headers
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RequestHeaderAttribute : RequestParameterTransformBaseAttribute
    {
        public RequestHeaderAttribute()
        {
        }

        public RequestHeaderAttribute(string name)
        {
            Name = name;
        }

        public string? Name { get; }

        public static KeyValuePair<string, string> GetHeader(string? name, string value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return new KeyValuePair<string, string>(name, value);
            }
            string[] values = value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (values == null || values.Length != 2)
            {
                throw new ArgumentException("value must be (key:value) when Name is empty", nameof(value));
            }
            return new KeyValuePair<string, string>(values[0], values[1]);
        }

        protected internal override LocalBuilder? EmitNewHttpRequestTransform(ILGenerator iLGenerator, LocalBuilder valueBuilder)
        {
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(IHttpRequestTransform));
            var method = typeof(RequestHeaderAttribute).GetMethod("GetHeader", BindingFlags.Public | BindingFlags.Static);
            //iLGenerator.Emit(OpCodes.Pop);
            //iLGenerator.Emit(OpCodes.Ldnull);
            if (!string.IsNullOrWhiteSpace(Name))
            {
                iLGenerator.Emit(OpCodes.Ldstr, Name);
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
