using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 指定返回的类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ResultTypeAttribute : Attribute
    {
        public ResultTypeAttribute(Type resultType)
        {
            if (resultType == null)
            {
                throw new ArgumentException(nameof(resultType));
            }
            ResultType = resultType;
        }

        public Type ResultType { get; }

        internal Type ConvertType(Type type)
        {
            Type resultType = ResultType;
            //特殊转换
            if (resultType.IsGenericType && resultType.IsGenericTypeDefinition && type.IsGenericType && resultType.GetGenericArguments().Length == type.GetGenericArguments().Length)
            {
                //补全
                resultType = resultType.MakeGenericType(type.GetGenericArguments());
            }

            if (type.IsAssignableFrom(resultType) || ImplicitlyConvertsTo(type, resultType))
            {
                return resultType;
            }
            return null;
        }

        private static bool ImplicitlyConvertsTo(Type type, Type destinationType)
        {
            if (type == destinationType)
                return true;

            return (from method in type.GetMethods(BindingFlags.Static |
                                                   BindingFlags.Public)
                    where method.Name == "op_Implicit" &&
                          method.ReturnType == destinationType
                    select method
                    ).Any();
        }

    }
}
