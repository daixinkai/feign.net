using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    public class FeignClientHttpProxyTypeBuilder : IFeignClientTypeBuilder
    {
        public FeignClientHttpProxyTypeBuilder() : this(new DynamicAssembly())
        {
        }

#if DEBUG&&NET45
        public
#else
        internal
#endif
        FeignClientHttpProxyTypeBuilder(DynamicAssembly dynamicAssembly)
        {
            _guid = Guid.NewGuid().ToString("N").ToUpper();
            _suffix = "_Proxy_" + _guid;
            _dynamicAssembly = dynamicAssembly;
            _methodBuilder = new FeignClientHttpProxyEmitMethodBuilder();
            _fallbackMethodBuilder = new FallbackFeignClientHttpProxyEmitMethodBuilder(_dynamicAssembly);
        }
        string _guid;
        string _suffix;

        FeignClientHttpProxyEmitMethodBuilder _methodBuilder;
        FallbackFeignClientHttpProxyEmitMethodBuilder _fallbackMethodBuilder;
        DynamicAssembly _dynamicAssembly;

        public FeignClientTypeInfo Build(Type serviceType)
        {
            if (!NeedBuildType(serviceType))
            {
                return null;
            }
            FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttribute<FeignClientAttribute>();

            IMethodBuilder methodBuilder;

            Type parentType;
            if (feignClientAttribute.Fallback != null)
            {
                methodBuilder = _fallbackMethodBuilder;
                parentType = typeof(FallbackFeignClientHttpProxy<,>);
                parentType = parentType.MakeGenericType(serviceType, feignClientAttribute.Fallback);
            }
            else if (feignClientAttribute.FallbackFactory != null)
            {
                methodBuilder = _fallbackMethodBuilder;
                parentType = typeof(FallbackFactoryFeignClientHttpProxy<,>);
                parentType = parentType.MakeGenericType(serviceType, feignClientAttribute.FallbackFactory);
            }
            else
            {
                methodBuilder = _methodBuilder;
                parentType = typeof(FeignClientHttpProxy<>);
                parentType = parentType.MakeGenericType(serviceType);
            }
            parentType = GetParentType(parentType);

            FeignClientTypeInfo feignClientTypeInfo = new FeignClientTypeInfo(serviceType)
            {
                ParentType = parentType
            };

            TypeBuilder typeBuilder = CreateTypeBuilder(GetTypeFullName(serviceType), parentType);

            BuildConstructor(typeBuilder, parentType);
            //serviceId
            BuildReadOnlyProperty(typeBuilder, serviceType, "ServiceId", serviceType.GetCustomAttribute<FeignClientAttribute>().Name);

            //baseUri
            BuildReadOnlyProperty(typeBuilder, serviceType, "BaseUri", serviceType.GetCustomAttribute<RequestMappingAttribute>()?.Value);

            // url
            if (serviceType.GetCustomAttribute<FeignClientAttribute>().Url != null)
            {
                BuildReadOnlyProperty(typeBuilder, serviceType, "Url", serviceType.GetCustomAttribute<FeignClientAttribute>().Url);
            }

            typeBuilder.AddInterfaceImplementation(serviceType);
            foreach (var method in serviceType.GetMethods())
            {
                methodBuilder.BuildMethod(typeBuilder, serviceType, method, feignClientAttribute);
            }
            var typeInfo = typeBuilder.CreateTypeInfo();
            Type type = typeInfo.AsType();

            feignClientTypeInfo.BuildType = type;

            return feignClientTypeInfo;
        }

        protected virtual Type GetParentType(Type parentType)
        {
            return parentType;
        }

        protected virtual ConstructorInfo GetConstructor(Type parentType)
        {
            return parentType.GetConstructors()[0];
        }

        void BuildConstructor(TypeBuilder typeBuilder, Type parentType)
        {
            ConstructorInfo baseConstructorInfo = GetConstructor(parentType);
            var parameterTypes = baseConstructorInfo.GetParameters().Select(s => s.ParameterType).ToArray();

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            for (int i = 1; i <= baseConstructorInfo.GetParameters().Length; i++)
            {
                constructorIlGenerator.Emit(OpCodes.Ldarg_S, i);
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseConstructorInfo);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        void BuildReadOnlyProperty(TypeBuilder typeBuilder, Type interfaceType, string propertyName, string propertyValue)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, typeof(string), Type.EmptyTypes);

            MethodBuilder propertyGet = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(string), Type.EmptyTypes);
            ILGenerator iLGenerator = propertyGet.GetILGenerator();
            if (propertyValue == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, propertyValue);
            }
            iLGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGet);
        }

        string GetTypeFullName(Type serviceType)
        {
            return serviceType.FullName + _suffix;
            //return interfaceType.Assembly.GetName().ToString() + "_" + interfaceType.FullName;
        }

        internal static bool NeedBuildType(Type type)
        {
            return type.IsInterface && type.IsDefined(typeof(FeignClientAttribute));
        }

        private TypeBuilder CreateTypeBuilder(string typeName, Type parentType)
        {
            return _dynamicAssembly.ModuleBuilder.DefineType(typeName,
                          TypeAttributes.Public |
                          TypeAttributes.Class |
                          TypeAttributes.AutoClass |
                          TypeAttributes.AnsiClass |
                          TypeAttributes.BeforeFieldInit |
                          TypeAttributes.AutoLayout,
                          parentType);
        }

#if DEBUG && NET45
        public void Save()
        {
            _dynamicAssembly.AssemblyBuilder.Save(_dynamicAssembly.AssemblyName);
        }
#endif

    }
}
