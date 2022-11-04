﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static partial class ReflectionExtensions
    {

        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
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
                    return type.FullName;
                }
                Dictionary<Type, string> cache = new Dictionary<Type, string>();
                return GetFullName(type, cache);
            }
            catch //(Exception ex)
            {
                return type.FullName;
            }
        }

        private static string GetFullName(Type type, Dictionary<Type, string> cache)
        {
            if (!type.IsGenericType)
            {
                return type.FullName;
            }
            string cacheFullName;
            if (cache.TryGetValue(type, out cacheFullName))
            {
                return cacheFullName;
            }
            string genericArgumentName = string.Join(",", type.GetGenericArguments().Select(s => GetFullName(s, cache)));
            string fullName = type.GetGenericTypeDefinition().FullName.Split(new char[] { '`' })[0];
            fullName = fullName + "<" + genericArgumentName + ">";
            cache.Add(type, fullName);
            return fullName;
        }

        public static bool IsDefinedIncludingBaseInterfaces<T>(this Type type)
        {
            return type.IsDefined(typeof(T)) || type.GetInterfaces().Any(s => IsDefinedIncludingBaseInterfaces<T>(s));
        }

        public static void BuildFirstConstructor(this TypeBuilder typeBuilder, Type parentType)
        {
            ConstructorInfo baseConstructorInfo = GetFirstConstructor(parentType);
            typeBuilder.BuildCallBaseTypeConstructor(baseConstructorInfo);
        }


        public static void BuildAndCallBaseTypeDefaultConstructor(this TypeBuilder typeBuilder)
        {
            Type baseType = (typeBuilder.BaseType ?? typeof(object));
            ConstructorInfo baseConstructorInfo = baseType.GetConstructors().Where(s => s.GetParameters().Length == 0).FirstOrDefault();
            if (baseConstructorInfo == null)
            {
                throw new ArgumentException("The default constructor not found . Type : " + baseType.FullName);
            }
            var parameterTypes = baseConstructorInfo.GetParameters().Select(s => s.ParameterType).ToArray();

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        public static void BuildAndCallBaseTypeConstructor(this TypeBuilder typeBuilder, Type[] baseConstructorParameterTypes)
        {
            ConstructorInfo baseConstructorInfo = (typeBuilder.BaseType ?? typeof(object)).GetConstructors().Where(s => Equals(s.GetParameters(), baseConstructorParameterTypes)).FirstOrDefault();
            if (baseConstructorInfo == null)
            {
                throw new ArgumentException("The constructor not found . Type[] : " + string.Join(",", baseConstructorParameterTypes.Select(s => s.FullName)));
            }
            typeBuilder.BuildCallBaseTypeConstructor(baseConstructorInfo);
        }

        public static void BuildCallBaseTypeConstructor(this TypeBuilder typeBuilder, ConstructorInfo baseConstructorInfo)
        {
            var parameters = baseConstructorInfo.GetParameters();
            var parameterTypes = parameters.Select(s => s.ParameterType).ToArray();
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

        public static void BuildConstructor(TypeBuilder typeBuilder, ConstructorInfo baseConstructor)
        {
            var parameterTypes = baseConstructor.GetParameters().Select(s => s.ParameterType).ToArray();

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                constructorBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameterTypes[i].Name);
            }
            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            for (int i = 1; i <= baseConstructor.GetParameters().Length; i++)
            {                
                constructorIlGenerator.EmitLdarg(i);
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseConstructor);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }



    }
}
