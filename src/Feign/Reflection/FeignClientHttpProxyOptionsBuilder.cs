using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    internal class FeignClientHttpProxyOptionsBuilder
    {
        public static Type BuildType(ModuleBuilder moduleBuilder, string guid, Type serviceType, Type? serviceConfigurationType, Type? configurationType)
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
            parameterTypes[parameterTypes.Count - 1] = serviceConfigurationType;
            BuildFirstConstructor(typeBuilder, parentType, parameterTypes.ToArray());
            return typeBuilder.CreateTypeInfo()!.AsType();
        }

        private static void BuildFirstConstructor(TypeBuilder typeBuilder, Type parentType, Type?[] arguments)
        {
            ConstructorInfo baseConstructorInfo = parentType.GetFirstConstructor();
            var baseParameters = baseConstructorInfo.GetParameters();
            var parameters = arguments.Where(s => s != null).ToArray();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameters!);
            int index = 0;
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
            CallBaseTypeConstructor(constructorIlGenerator, baseConstructorInfo, arguments);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        private static void CallBaseTypeConstructor(ILGenerator constructorIlGenerator, ConstructorInfo baseTypeConstructor, Type?[] arguments)
        {
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            int index = 1;
            for (int i = 1; i <= baseTypeConstructor.GetParameters().Length; i++)
            {
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
