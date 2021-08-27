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
    /// 将Authorization设置到请求头中
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RequestAuthorizationAttribute : RequestHeaderBaseAttribute
    {
        public RequestAuthorizationAttribute()
        {
        }

        public RequestAuthorizationAttribute(string scheme)
        {
            Scheme = scheme;
        }

        public string Scheme { get; }


        public static KeyValuePair<string, string> GetHeader(string scheme, string value)
        {
            if (!string.IsNullOrWhiteSpace(scheme))
            {
                return new KeyValuePair<string, string>("Authorization", scheme + " " + value);
            }
            return new KeyValuePair<string, string>("Authorization", value);
            //string[] values = value?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //if (values.Length != 2)
            //{
            //    throw new ArgumentException("value must be (scheme value) when Name is empty", nameof(value));
            //}
            //return new KeyValuePair<string, string>("Authorization", values[0] + " " + values[1]);
        }

        protected internal override LocalBuilder EmitNewRequestHeaderHandler(ILGenerator iLGenerator, LocalBuilder valueBuilder)
        {
            return null;
            var method = typeof(RequestAuthorizationAttribute).GetMethod("GetHeader", BindingFlags.Public | BindingFlags.Static);

            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                iLGenerator.Emit(OpCodes.Ldstr, Scheme);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }            
            iLGenerator.Emit(OpCodes.Ldloc, valueBuilder);
            iLGenerator.Emit(OpCodes.Callvirt, method);
            iLGenerator.Emit(OpCodes.Pop);
            iLGenerator.Emit(OpCodes.Newobj, typeof(RequestHeaderHandler).GetFirstConstructor());
            //if (!string.IsNullOrWhiteSpace(Scheme))
            //{
            //    iLGenerator.Emit(OpCodes.Ldstr, Scheme);
            //    iLGenerator.Emit(OpCodes.Ldloc, valueBuilder);
            //    iLGenerator.Emit(OpCodes.Newobj, typeof(RequestHeaderHandler).GetFirstConstructor());
            //    //return new DefaultRequestHeaderHandler("Authorization", scheme + " " + value);
            //}
            //else
            //{
            //    iLGenerator.Emit(OpCodes.Ldstr, "Authorization");
            //    iLGenerator.Emit(OpCodes.Ldloc, valueBuilder);
            //    iLGenerator.Emit(OpCodes.Newobj, typeof(RequestHeaderHandler).GetFirstConstructor());
            //    //return new DefaultRequestHeaderHandler("Authorization", value);
            //    //string[] values = value?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    //if (values.Length != 2)
            //    //{
            //    //    throw new ArgumentException("value must be (scheme value) when Name is empty", nameof(value));
            //    //}
            //    //return new DefaultRequestHeaderHandler("Authorization", values[0] + " " + values[1]);
            //}
            return null;
        }

    }
}
