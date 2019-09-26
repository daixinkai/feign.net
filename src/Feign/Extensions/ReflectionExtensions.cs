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

        //public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        //{
        //    return type.GetCustomAttributes<T>().
        //      Union(type.GetInterfaces().
        //      SelectMany(interfaceType => interfaceType.GetCustomAttributes<T>())).
        //      Distinct();
        //}

        //public static T GetCustomAttributeIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        //{
        //    return type.GetCustomAttributesIncludingBaseInterfaces<T>().FirstOrDefault();
        //}


        public static T GetCustomAttributeIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
            {
                return attribute;
            }
            return GetCustomAttributeFromBaseInterfaces<T>(type);
        }

        static T GetCustomAttributeFromBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T attribute = null;
            foreach (var item in type.GetInterfaces())
            {
                attribute = item.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                attribute = GetCustomAttributeFromBaseInterfaces<T>(item);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }


        public static bool IsDefinedIncludingBaseInterfaces<T>(this Type type)
        {
            return type.IsDefined(typeof(T)) || type.GetInterfaces().Any(s => IsDefinedIncludingBaseInterfaces<T>(s));
        }

        public static MethodInfo[] GetMethodsIncludingBaseInterfaces(this Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods());
            GetMethodsFromBaseInterfaces(type, methods);
            return methods.ToArray();
        }

        static void GetMethodsFromBaseInterfaces(this Type type, List<MethodInfo> methods)
        {
            foreach (var item in type.GetInterfaces())
            {
                foreach (var method in item.GetMethods())
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
