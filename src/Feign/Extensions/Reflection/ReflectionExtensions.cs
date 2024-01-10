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
    internal static partial class ReflectionExtensions
    {

        public static ConstructorInfo? GetEmptyConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static ConstructorInfo? GetDefaultConstructor(this Type type)
        {
            //return type.GetConstructors().Where(s => s.GetParameters().Length == 0).FirstOrDefault();
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static ConstructorInfo GetFirstConstructor(this Type type)
        {
            return type.GetConstructors()[0];
        }

        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static string GetFullName(this Type type)
        {
            try
            {
                if (!type.IsGenericType)
                {
                    return type.FullName!;
                }
                Dictionary<Type, string> cache = new();
                return GetFullName(type, cache);
            }
            catch //(Exception ex)
            {
                return type.FullName!;
            }
        }

        private static string GetFullName(Type type, Dictionary<Type, string> cache)
        {
            if (!type.IsGenericType)
            {
                return type.FullName!;
            }
            if (cache.TryGetValue(type, out var cacheFullName))
            {
                return cacheFullName;
            }
            string genericArgumentName = string.Join(",", type.GetGenericArguments().Select(s => GetFullName(s, cache)));
            string fullName = type.GetGenericTypeDefinition().FullName!.Split(new char[] { '`' })[0];
            fullName = fullName + "<" + genericArgumentName + ">";
            cache.Add(type, fullName);
            return fullName;
        }

        public static bool IsDefinedIncludingBaseInterfaces<T>(this Type type)
        {
            return type.IsDefined(typeof(T)) || type.GetInterfaces().Any(IsDefinedIncludingBaseInterfaces<T>);
        }

        public static void BuildFirstConstructor(this TypeBuilder typeBuilder, Type parentType)
        {
            ConstructorInfo baseConstructorInfo = GetFirstConstructor(parentType);
            var parameters = baseConstructorInfo.GetParameters();
            var parameterTypes = parameters.Select(static s => s.ParameterType).ToArray();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);
            for (int i = 0; i < parameters.Length; i++)
            {
                constructorBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
            }
            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        public static void BuildFirstConstructor(this TypeBuilder typeBuilder, Type parentType, Dictionary<Type, Type> replaceArguments)
        {
            ConstructorInfo baseConstructorInfo = GetFirstConstructor(parentType);
            var parameters = baseConstructorInfo.GetParameters();
            var parameterTypes = parameters.Select(s =>
            {
                if (replaceArguments.TryGetValue(s.ParameterType, out var parameterType))
                {
                    return parameterType;
                }
                return s.ParameterType;
            }).ToArray();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);
            for (int i = 0; i < parameters.Length; i++)
            {
                constructorBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
            }
            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        public static MethodInfo GetRequiredMethod(this Type type, string name)
            => type.GetMethod(name)!;
        public static MethodInfo GetRequiredMethod(this Type type, string name, BindingFlags bindingAttr)
            => type.GetMethod(name, bindingAttr)!;
        public static MethodInfo GetRequiredMethod(this Type type, string name, Type[] types)
            => type.GetMethod(name, types)!;

        public static PropertyInfo GetRequiredProperty(this Type type, string name)
            => type.GetProperty(name)!;
        public static PropertyInfo GetRequiredProperty(this Type type, string name, BindingFlags bindingAttr)
            => type.GetProperty(name, bindingAttr)!;

    }
}
