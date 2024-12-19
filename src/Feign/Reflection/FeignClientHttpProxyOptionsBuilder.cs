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
        class ConstructorParameter
        {
            public int? Index { get; set; }
            public string Name { get; set; } = null!;
            public PropertyInfo? Property { get; set; }
            public Type Type { get; set; } = null!;
            public override bool Equals(object? obj)
            {
                return Name.Equals(((ConstructorParameter)obj!).Name);
            }
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public static ConstructorParameter Create(ParameterInfo parameterInfo)
                => Create(null, parameterInfo.Name!, null, parameterInfo.ParameterType);

            public static ConstructorParameter Create(int? index, string name, PropertyInfo? property, Type type)
                => new ConstructorParameter
                {
                    Index = index,
                    Name = name,
                    Property = property,
                    Type = type
                };
        }

        public static Type BuildType(ModuleBuilder moduleBuilder, string guid, Type configurationType)
        {
            var parentType = typeof(FeignClientHttpProxyOptions);
            //创建类型
            TypeAttributes typeAttributes = TypeAttributes.Public |
                     TypeAttributes.Class |
                     TypeAttributes.AutoClass |
                     TypeAttributes.AnsiClass |
                     TypeAttributes.BeforeFieldInit |
                     TypeAttributes.AutoLayout;
            string typeName = configurationType.Name;
            typeName += "_" + guid;
            typeName = configurationType.Namespace + ".ProxyOptions." + typeName;
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, Type.EmptyTypes);

            var configurationProperty = parentType.GetProperty("Configuration");
            var serviceConfigurationProperty = parentType.GetProperty("ServiceConfiguration");

            ConstructorInfo baseConstructorInfo = parentType.GetFirstConstructor();

            var constructorParameters = baseConstructorInfo.GetParameters().Select(ConstructorParameter.Create).ToList();

            constructorParameters.Add(ConstructorParameter.Create(null, "configuration", configurationProperty, configurationType));

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               constructorParameters.Distinct().Select(static s => s.Type).ToArray());

            int index = 0;
            foreach (var key in constructorParameters.Distinct())
            {
                index++;
                constructorBuilder.DefineParameter(index, ParameterAttributes.None, key.Name);
            }

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);

            index = 0;
            foreach (var constructorParameter in constructorParameters)
            {
                index++;
                if (constructorParameter.Property == null)
                {
                    continue;
                }
                constructorIlGenerator.Emit(OpCodes.Ldarg_0);
                constructorIlGenerator.EmitLdargS(constructorParameter.Index ?? index);
                constructorIlGenerator.EmitSetProperty(constructorParameter.Property);
            }
            constructorIlGenerator.Emit(OpCodes.Ret);
            return typeBuilder.CreateTypeInfo()!.AsType();
        }

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

            var configurationProperty = parentType.GetProperty("Configuration");
            var serviceConfigurationProperty = parentType.GetProperty("ServiceConfiguration");

            ConstructorInfo baseConstructorInfo = parentType.GetFirstConstructor();

            var constructorParameters = baseConstructorInfo.GetParameters().Select(ConstructorParameter.Create).ToList();

            if (configurationType != null && configurationType == serviceConfigurationType)
            {
                constructorParameters.Add(ConstructorParameter.Create(constructorParameters.Count + 1, "configuration", configurationProperty, configurationType));
                constructorParameters.Add(ConstructorParameter.Create(constructorParameters.Count, "configuration", serviceConfigurationProperty, configurationType));
            }
            else
            {
                if (configurationType != null)
                {
                    constructorParameters.Add(ConstructorParameter.Create(null, "configuration", configurationProperty, configurationType));
                }
                else if (serviceConfigurationType != null)
                {
                    constructorParameters.Add(ConstructorParameter.Create(null, "serviceConfiguration", serviceConfigurationProperty, serviceConfigurationType));
                }
            }

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               constructorParameters.Distinct().Select(static s => s.Type).ToArray());

            int index = 0;
            foreach (var key in constructorParameters.Distinct())
            {
                index++;
                constructorBuilder.DefineParameter(index, ParameterAttributes.None, key.Name);
            }

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);

            index = 0;
            foreach (var constructorParameter in constructorParameters)
            {
                index++;
                if (constructorParameter.Property == null)
                {
                    continue;
                }
                constructorIlGenerator.Emit(OpCodes.Ldarg_0);
                constructorIlGenerator.EmitLdargS(constructorParameter.Index ?? index);
                constructorIlGenerator.EmitSetProperty(constructorParameter.Property);
            }
            constructorIlGenerator.Emit(OpCodes.Ret);
            return typeBuilder.CreateTypeInfo()!.AsType();
        }

    }
}
