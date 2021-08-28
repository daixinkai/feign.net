using Feign.Request.Headers;
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
    /// 将参数转换到请求头中
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RequestHeaderAttribute : RequestHeaderBaseAttribute
    {
        public RequestHeaderAttribute()
        {
        }

        public RequestHeaderAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static KeyValuePair<string, string> GetHeader(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return new KeyValuePair<string, string>(name, value);
            }
            string[] values = value?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
            {
                throw new ArgumentException("value must be (key:value) when Name is empty", nameof(value));
            }
            return new KeyValuePair<string, string>(values[0], values[1]);
        }

        protected internal override LocalBuilder EmitNewRequestHeaderHandler(ILGenerator iLGenerator, LocalBuilder valueBuilder)
        {
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(IRequestHeaderHandler));
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
            iLGenerator.Emit(OpCodes.Newobj, typeof(RequestHeaderHandler).GetFirstConstructor());
            iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            return localBuilder;
        }

    }
}
