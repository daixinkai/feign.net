using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class ReflectionExtensions
    {
        public static bool IsVoidMethod(this MethodInfo method)
        {
            return method.ReturnType == null || method.ReturnType == typeof(void);
        }

        public static bool IsTaskMethod(this MethodInfo method)
        {
            return method.ReturnType == typeof(Task) || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        //public static Type GetReturnType(this MethodInfo method)
        //{
        //    if (!IsTaskMethod(method))
        //    {
        //        return method.ReturnType;
        //    }

        //    if (method.ReturnType.IsGenericType)
        //    {
        //        return method.ReturnType.GetGenericArguments()[0];
        //    }
        //    return null;
        //    //return method.ReturnType;
        //}

    }
}
