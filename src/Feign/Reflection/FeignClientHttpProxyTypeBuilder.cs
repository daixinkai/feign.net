using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    /// <summary>
    /// 默认的代理类型生成器
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
        string _guid;
        string _suffix;

        FeignClientHttpProxyEmitMethodBuilder _methodBuilder;
        FallbackFeignClientHttpProxyEmitMethodBuilder _fallbackMethodBuilder;
        DynamicAssembly _dynamicAssembly;

        public FeignClientTypeInfo Build(Type serviceType)
        {
            //校验一下是否可以生成代理类型
            if (!NeedBuildType(serviceType))
            {
                return null;
            }
            //获取一下描述特性

            FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttributeIncludingBaseInterfaces<FeignClientAttribute>();

            IMethodBuilder methodBuilder;

            Type parentType;
            if (feignClientAttribute.Fallback != null)
            {
                //此服务支持服务降级
                methodBuilder = _fallbackMethodBuilder;
                parentType = typeof(FallbackFeignClientHttpProxy<,>);
                parentType = parentType.MakeGenericType(serviceType, feignClientAttribute.Fallback);
            }
            else if (feignClientAttribute.FallbackFactory != null)
            {
                //此服务支持服务降级(降级服务由factory提供)
                methodBuilder = _fallbackMethodBuilder;
                parentType = typeof(FallbackFactoryFeignClientHttpProxy<,>);
                parentType = parentType.MakeGenericType(serviceType, feignClientAttribute.FallbackFactory);
            }
            else
            {
                //默认的服务
                methodBuilder = _methodBuilder;
                parentType = typeof(FeignClientHttpProxy<>);
                parentType = parentType.MakeGenericType(serviceType);
            }
            parentType = GetParentType(parentType);

            FeignClientTypeInfo feignClientTypeInfo = new FeignClientTypeInfo(serviceType)
            {
                ParentType = parentType
            };

            //创建类型
            TypeAttributes typeAttributes = TypeAttributes.Public |
                     TypeAttributes.Class |
                     TypeAttributes.AutoClass |
                     TypeAttributes.AnsiClass |
                     TypeAttributes.BeforeFieldInit |
                     TypeAttributes.AutoLayout;

            TypeBuilder typeBuilder = _dynamicAssembly.DefineType(GetTypeFullName(serviceType), typeAttributes, parentType, new Type[] { serviceType });

            //写入构造函数
            typeBuilder.BuildFirstConstructor(parentType);

            //写入serviceId
            typeBuilder.DefineReadOnlyProperty(serviceType, "ServiceId", feignClientAttribute.Name);

            //写入baseUri
            typeBuilder.DefineReadOnlyProperty(serviceType, "BaseUri", serviceType.GetCustomAttribute<RequestMappingAttribute>()?.Value);

            // 写入url
            if (feignClientAttribute.Url != null)
            {
                typeBuilder.DefineReadOnlyProperty(serviceType, "Url", feignClientAttribute.Url);
            }

            foreach (var method in serviceType.GetMethodsIncludingBaseInterfaces())
            {
                //生成方法
                var buildMethod = methodBuilder.BuildMethod(typeBuilder, serviceType, method, feignClientAttribute);
                feignClientTypeInfo.Methods.Add(buildMethod);
            }

            foreach (var property in serviceType.GetPropertiesIncludingBaseInterfaces())
            {
                //写入自动属性
                //typeBuilder.DefineAutoProperty(serviceType, property);
                typeBuilder.DefineExplicitAutoProperty(serviceType, property);
            }

            var typeInfo = typeBuilder.CreateTypeInfo();
            Type type = typeInfo.AsType();

            feignClientTypeInfo.BuildType = type;

            return feignClientTypeInfo;
        }
        /// <summary>
        /// 获取服务的父类型
        /// </summary>
        /// <param name="parentType"></param>
        /// <returns></returns>
        protected virtual Type GetParentType(Type parentType)
        {
            return parentType;
        }


        string GetTypeFullName(Type serviceType)
        {
            return serviceType.FullName + _suffix;
            //return interfaceType.Assembly.GetName().ToString() + "_" + interfaceType.FullName;
        }

        internal static bool NeedBuildType(Type type)
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
