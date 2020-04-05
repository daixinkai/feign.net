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
    static partial class ReflectionExtensions
    {        

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

        static string GetFullName(Type type, Dictionary<Type, string> cache)
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

        static ConstructorInfo GetFirstConstructor(Type parentType)
        {
            return parentType.GetConstructors()[0];
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

        static bool Equals(Type[] types1, Type[] types2)
        {
            if (types1.Length != types2.Length)
            {
                return false;
            }

            for (int i = 0; i < types1.Length; i++)
            {
                var type1 = types1[i];
                var type2 = types2[i];
                //if (type1.IsArray != type2.IsArray)
                //{
                //    return false;
                //}
                if (types1[i] != types2[i])
                {
                    return false;
                }
            }
            return true;
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
                constructorIlGenerator.Emit(OpCodes.Ldarg_S, i);
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseConstructor);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }



    }
}
