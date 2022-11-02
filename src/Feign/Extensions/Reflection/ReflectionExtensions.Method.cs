using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    partial class ReflectionExtensions
    {

        public static bool IsVoidMethod(this MethodInfo method)
        {
            return method.ReturnType == null || method.ReturnType == typeof(void);
        }

        public static bool IsTaskMethod(this MethodInfo method)
        {
            return method.ReturnType == typeof(Task) || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

//#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
//        public static bool IsValueTaskMethod(this MethodInfo method)
//        {
//            return method.ReturnType == typeof(ValueTask) || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>);
//        }
//#endif

        public static MethodInfo[] GetMethodsIncludingBaseInterfaces(this Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods().Where(s => !s.IsSpecialName));
            GetMethodsFromBaseInterfaces(type, methods);
            return methods.ToArray();
        }

        private static void GetMethodsFromBaseInterfaces(this Type type, List<MethodInfo> methods)
        {
            foreach (var item in type.GetInterfaces())
            {
                foreach (var method in item.GetMethods().Where(s => !s.IsSpecialName))
                {
                    if (!methods.Contains(method))
                    {
                        methods.Add(method);
                    }
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                GetMethodsFromBaseInterfaces(item, methods);
            }
        }

    }
}
