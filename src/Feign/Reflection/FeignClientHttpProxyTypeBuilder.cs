using Feign.Configuration;
using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Feign.Reflection
{
    /// <summary>
    /// Default proxy type generator
    /// </summary>
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
        private readonly string _guid;
        private readonly string _suffix;

        private readonly FeignClientHttpProxyEmitMethodBuilder _methodBuilder;
        private readonly FallbackFeignClientHttpProxyEmitMethodBuilder _fallbackMethodBuilder;
        private readonly DynamicAssembly _dynamicAssembly;

        private readonly Dictionary<Type, Type> _configurationTypeMap = new();

        public FeignClientTypeInfo? Build(Type serviceType)
        {
            // Check whether the proxy type can be generated
            if (!NeedBuildType(serviceType))
            {
                return null;
            }

            // get Attribute

            FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttributeIncludingBaseInterfaces<FeignClientAttribute>()!;



            Type parentType = GetParentType(serviceType, feignClientAttribute);
            var methodBuilder = GetMethodBuilder(serviceType, parentType, feignClientAttribute);


            var feignClientTypeInfo = new FeignClientTypeInfo(feignClientAttribute, serviceType)
            {
                ParentType = parentType
            };

            var feignClientHttpProxyOptionsType = BuildFeignClientHttpProxyOptionsType(serviceType, feignClientAttribute);

            //new type
            TypeAttributes typeAttributes = TypeAttributes.Public |
                     TypeAttributes.Class |
                     TypeAttributes.AutoClass |
                     TypeAttributes.AnsiClass |
                     TypeAttributes.BeforeFieldInit |
                     TypeAttributes.AutoLayout;

            TypeBuilder typeBuilder = _dynamicAssembly.DefineType(GetTypeFullName(serviceType), typeAttributes, parentType, new Type[] { serviceType });

            // write Constructor
            if (feignClientHttpProxyOptionsType == null)
            {
                typeBuilder.BuildFirstConstructor(parentType);
            }
            else
            {
                feignClientTypeInfo.ProxyOptionsType = new FeignClientProxyOptionsTypeInfo(feignClientHttpProxyOptionsType, feignClientAttribute.Configuration!);
                typeBuilder.BuildFirstConstructor(parentType, new Dictionary<Type, Type>
                {
                    [typeof(FeignClientHttpProxyOptions)] = feignClientHttpProxyOptionsType
                });
            }


            // write serviceId
            typeBuilder.OverrideProperty(typeBuilder.BaseType!.GetRequiredProperty("ServiceId"), iLGenerator =>
            {
                iLGenerator.EmitStringValue(feignClientAttribute.Name);
                iLGenerator.Emit(OpCodes.Ret);
            }, null);
            // write baseUri
            typeBuilder.OverrideProperty(typeBuilder.BaseType!.GetRequiredProperty("BaseUri"), iLGenerator =>
            {
                var value = serviceType.GetCustomAttribute<RequestMappingAttribute>()?.Value;
                iLGenerator.EmitStringValue(value);
                iLGenerator.Emit(OpCodes.Ret);
            }, null);
            // override UriKind
            typeBuilder.OverrideProperty(typeBuilder.BaseType!.GetRequiredProperty("UriKind", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), iLGenerator =>
            {
                iLGenerator.EmitEnumValue(feignClientAttribute.UriKind);
                iLGenerator.Emit(OpCodes.Ret);
            }, null);

            // write url
            if (feignClientAttribute.Url != null)
            {
                typeBuilder.OverrideProperty(typeBuilder.BaseType!.GetRequiredProperty("Url"), iLGenerator =>
                {
                    if (feignClientAttribute.Url == null)
                    {
                        iLGenerator.Emit(OpCodes.Ldnull);
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldstr, feignClientAttribute.Url);
                    }
                    iLGenerator.Emit(OpCodes.Ret);
                }, null);
            }

            // headers
            if (serviceType.IsDefined(typeof(HeadersAttribute), true))
            {
                var headersAttribute = serviceType.GetCustomAttribute<HeadersAttribute>();
                var headersFieldBuilder = typeBuilder.DefineField("s_headers", typeof(string[]), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
                #region cctor
                var cctor = typeBuilder.DefineConstructor(MethodAttributes.PrivateScope | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static, CallingConventions.Standard, Type.EmptyTypes);
                var cctorILGenerator = cctor.GetILGenerator();
                List<EmitStringValue>? headers = headersAttribute!.Headers?.Select(static s => new EmitStringValue(s)).ToList();
                if (headers != null)
                {
                    cctorILGenerator.EmitNewArray(headers);
                    cctorILGenerator.Emit(OpCodes.Stsfld, headersFieldBuilder);
                }
                cctorILGenerator.Emit(OpCodes.Ret);
                #endregion
                //重写DefaultHeaders
                typeBuilder.OverrideProperty(typeBuilder.BaseType!.GetRequiredProperty("DefaultHeaders", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), iLGenerator =>
                {
                    iLGenerator.Emit(OpCodes.Ldsfld, headersFieldBuilder);
                    iLGenerator.Emit(OpCodes.Ret);
                }, null);
            }


            foreach (var method in serviceType.GetMethodsIncludingBaseInterfaces())
            {
                if (!method.Attributes.HasFlag(MethodAttributes.Abstract))
                {
                    //is default interface method?
                    //https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/proposals/csharp-8.0/default-interface-methods
                    continue;
                }
                // build method
                var buildMethod = methodBuilder.BuildMethod(typeBuilder, serviceType, method, feignClientAttribute);
                feignClientTypeInfo.Methods.Add(buildMethod);
            }

            foreach (var property in serviceType.GetPropertiesIncludingBaseInterfaces())
            {
                // write auto property
                //typeBuilder.DefineAutoProperty(serviceType, property);
                typeBuilder.DefineExplicitAutoProperty(property);
            }


            var typeInfo = typeBuilder.CreateTypeInfo();
            Type type = typeInfo!.AsType();

            feignClientTypeInfo.BuildType = type;

            return feignClientTypeInfo;

        }

        /// <summary>
        /// get parent type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="feignClientAttribute"></param>
        /// <returns></returns>
        protected virtual Type GetParentType(Type serviceType, FeignClientAttribute feignClientAttribute)
        {
            if (feignClientAttribute.Fallback != null)
            {
                // This service supports service fallback
                return typeof(FallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, feignClientAttribute.Fallback);
            }
            else if (feignClientAttribute.FallbackFactory != null)
            {
                // This service supports service fallback(from fallback factory)
                return typeof(FallbackFactoryFeignClientHttpProxy<,>).MakeGenericType(serviceType, feignClientAttribute.FallbackFactory);
            }
            else
            {
                //default service
                return typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType);
            }
        }

        /// <summary>
        /// get method builder
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="parentType"></param>
        /// <param name="feignClientAttribute"></param>
        /// <returns></returns>
        protected virtual IMethodBuilder GetMethodBuilder(Type serviceType, Type parentType, FeignClientAttribute feignClientAttribute)
        {
            if (feignClientAttribute.Fallback != null)
            {
                // This service supports service fallback
                return _fallbackMethodBuilder;
            }
            else if (feignClientAttribute.FallbackFactory != null)
            {
                // This service supports service fallback(from fallback factory)
                return _fallbackMethodBuilder;
            }
            //default service
            return _methodBuilder;
        }

        private Type? BuildFeignClientHttpProxyOptionsType(Type serviceType, FeignClientAttribute feignClientAttribute)
        {
            if (feignClientAttribute.Configuration == null)
            {
                return null;
            }
            if (feignClientAttribute.Configuration.IsInterface || feignClientAttribute.Configuration.IsAbstract /*|| !feignClientAttribute.Configuration.IsPublic*/)
            {
                return null;
            }
            Type? configurationType = null;
            Type? serviceConfigurationType = null;
            if (typeof(IFeignClientConfiguration<>).MakeGenericType(serviceType).IsAssignableFrom(feignClientAttribute.Configuration))
            {
                serviceConfigurationType = feignClientAttribute.Configuration;
            }
            if (typeof(IFeignClientConfiguration).IsAssignableFrom(feignClientAttribute.Configuration))
            {
                configurationType = feignClientAttribute.Configuration;
            }

            if (serviceConfigurationType != null)
            {
                return FeignClientHttpProxyOptionsBuilder.BuildType(_dynamicAssembly.ModuleBuilder, _guid, serviceType, configurationType, serviceConfigurationType);
            }

            if (configurationType == null)
            {
                return null;
            }

            if (!_configurationTypeMap.TryGetValue(feignClientAttribute.Configuration, out var optionsType))
            {
                optionsType = FeignClientHttpProxyOptionsBuilder.BuildType(_dynamicAssembly.ModuleBuilder, _guid, feignClientAttribute.Configuration);
                _configurationTypeMap[feignClientAttribute.Configuration] = optionsType;
            }
            return optionsType;

        }

        private string GetTypeFullName(Type serviceType)
        {
            return serviceType.FullName + _suffix;
            //return interfaceType.Assembly.GetName().ToString() + "_" + interfaceType.FullName;
        }

        private static bool NeedBuildType(Type type)
        {
            return type.IsInterface && type.IsDefinedIncludingBaseInterfaces<FeignClientAttribute>() && !type.IsDefined(typeof(NonFeignClientAttribute)) && !type.IsGenericType;
        }

#if DEBUG && NET45
        public void Save()
        {
            _dynamicAssembly.AssemblyBuilder.Save(_dynamicAssembly.AssemblyName);
        }
#endif

    }
}
