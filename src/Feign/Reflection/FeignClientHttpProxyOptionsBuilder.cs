using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Feign.Configuration;

namespace Feign.Reflection
{
    internal class FeignClientHttpProxyOptionsBuilder
    {
        public static Type BuildType(ModuleBuilder moduleBuilder, string guid, Type serviceType, Type? configurationType, Type? serviceConfigurationType)
        {
            var parentType = typeof(FeignClientHttpProxyOptions<>).MakeGenericType(serviceType);
            //创建类型
            TypeAttributes typeAttributes = TypeAttributes.Public |
                     TypeAttributes.Class |
                     TypeAttributes.AutoClass |
                     TypeAttributes.AnsiClass |
                     TypeAttributes.BeforeFieldInit |
                     TypeAttributes.AutoLayout;
            string typeName = serviceType.Name;
            typeName += "_" + guid;
            typeName = serviceType.Namespace + ".ProxyOptions." + typeName;
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, Type.EmptyTypes);
            List<Type?> parameterTypes = parentType.GetFirstConstructor().GetParameters().Select(s => s.ParameterType).ToList()!;
            parameterTypes[parameterTypes.Count - 2] = configurationType;
            if (configurationType == null || configurationType != serviceConfigurationType)
            {
                parameterTypes[parameterTypes.Count - 1] = serviceConfigurationType;
            }
            else
            {
                parameterTypes[parameterTypes.Count - 1] = null;
            }
            BuildFirstConstructor(typeBuilder, parentType, parameterTypes.ToArray(), configurationType, serviceConfigurationType);
            return typeBuilder.CreateTypeInfo()!.AsType();
        }

        private static void BuildFirstConstructor(TypeBuilder typeBuilder, Type parentType, Type?[] arguments, Type? configurationType, Type? serviceConfigurationType)
        {
            ConstructorInfo baseConstructorInfo = parentType.GetFirstConstructor();
            var baseParameters = baseConstructorInfo.GetParameters();
            var parameters = arguments.Where(s => s != null).ToArray();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameters!);
            int index = 0;
            List<Type> types = new List<Type>();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                {
                    continue;
                }
                constructorBuilder.DefineParameter(index + 1, ParameterAttributes.None, baseParameters[i].Name);
                index++;
            }
            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            CallBaseTypeConstructor(constructorIlGenerator, baseConstructorInfo, arguments, configurationType, serviceConfigurationType);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        private static void CallBaseTypeConstructor(ILGenerator constructorIlGenerator, ConstructorInfo baseTypeConstructor, Type?[] arguments, Type? configurationType, Type? serviceConfigurationType)
        {
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            int index = 1;
            var parameters = baseTypeConstructor.GetParameters();
            for (int i = 1; i <= parameters.Length; i++)
            {
                var parameter = parameters[i - 1];
                if (parameter.ParameterType == typeof(IFeignClientConfiguration) || (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(IFeignClientConfiguration<>)))
                {
                    if (configurationType != null && configurationType == serviceConfigurationType)
                    {
                        constructorIlGenerator.EmitLdarg(index);
                        continue;
                    }
                }
                if (arguments[i - 1] == null)
                {
                    constructorIlGenerator.Emit(OpCodes.Ldnull);
                    continue;
                }
                constructorIlGenerator.EmitLdarg(index);
                index++;
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseTypeConstructor);
        }


    }
}
