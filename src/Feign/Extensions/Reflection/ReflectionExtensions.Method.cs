using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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


        public static bool IsValueTaskMethod(this MethodInfo method)
        {
#if USE_VALUE_TASK
            return method.ReturnType == typeof(ValueTask) || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>);
#else
            return false;
#endif
        }

        public static MethodInfo[] GetMethodsIncludingBaseInterfaces(this Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods().Where(static s => !s.IsSpecialName));
            GetMethodsFromBaseInterfaces(type, methods);
            return methods.ToArray();
        }

        private static void GetMethodsFromBaseInterfaces(this Type type, List<MethodInfo> methods)
        {
            foreach (var item in type.GetInterfaces())
            {
                foreach (var method in item.GetMethods().Where(static s => !s.IsSpecialName))
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

        public static MethodBuilder DefineMethodEx(this TypeBuilder typeBuilder, MethodInfo method, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, attributes, callingConvention, returnType, parameterTypes);
            if (method.IsGenericMethod && method.IsGenericMethodDefinition)
            {
                var names = new List<string>();
                var length = method.GetGenericArguments().Length;
                for (int i = 1; i <= length; i++)
                {
                    names.Add("T" + i);
                }
                methodBuilder.DefineGenericParameters(names.ToArray());
            }
            return methodBuilder;
        }

        public static MethodInfo MakeGenericDefinitionArguments(this MethodInfo method, MethodBuilder methodBuilder)
        {
            if (method.IsGenericMethod && method.IsGenericMethodDefinition)
            {
                return method.MakeGenericMethod(methodBuilder.GetGenericArguments());
            }
            return method;
        }

    }
}
