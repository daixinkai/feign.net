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
            //FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttribute<FeignClientAttribute>();
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
            TypeBuilder typeBuilder = CreateTypeBuilder(GetTypeFullName(serviceType), parentType);
            //写入构造函数
            BuildConstructor(typeBuilder, parentType);

            //写入serviceId
            BuildReadOnlyProperty(typeBuilder, serviceType, "ServiceId", feignClientAttribute.Name);

            //写入baseUri
            BuildReadOnlyProperty(typeBuilder, serviceType, "BaseUri", serviceType.GetCustomAttribute<RequestMappingAttribute>()?.Value);

            // 写入url
            if (feignClientAttribute.Url != null)
            {
                BuildReadOnlyProperty(typeBuilder, serviceType, "Url", feignClientAttribute.Url);
            }
            //生成的类型必须实现服务
            typeBuilder.AddInterfaceImplementation(serviceType);
            //foreach (var method in serviceType.GetMethods())
            foreach (var method in serviceType.GetMethodsIncludingBaseInterfaces())
            {
                //生成方法
                var buildMethod = methodBuilder.BuildMethod(typeBuilder, serviceType, method, feignClientAttribute);
                feignClientTypeInfo.Methods.Add(buildMethod);
            }

            foreach (var property in serviceType.GetPropertiesIncludingBaseInterfaces())
            {
                BuildAutoProperty(typeBuilder, serviceType, property);
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

        protected virtual ConstructorInfo GetConstructor(Type parentType)
        {
            return parentType.GetConstructors()[0];
        }

        void BuildConstructor(TypeBuilder typeBuilder, Type parentType)
        {
            ConstructorInfo baseConstructorInfo = GetConstructor(parentType);
            typeBuilder.BuildCallBaseTypeConstructor(baseConstructorInfo);
            //var parameterTypes = baseConstructorInfo.GetParameters().Select(s => s.ParameterType).ToArray();

            //ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
            //   MethodAttributes.Public,
            //   CallingConventions.Standard,
            //   parameterTypes);

            //ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            //constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            //for (int i = 1; i <= baseConstructorInfo.GetParameters().Length; i++)
            //{
            //    constructorIlGenerator.Emit(OpCodes.Ldarg_S, i);
            //}
            //constructorIlGenerator.Emit(OpCodes.Call, baseConstructorInfo);
            //constructorIlGenerator.Emit(OpCodes.Ret);
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

        void BuildAutoProperty(TypeBuilder typeBuilder, Type type, PropertyInfo property)
        {

            MethodAttributes methodAttributes =
                MethodAttributes.Public
                | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final;

            string fieldName = "<" + property.Name + ">k__BackingField";
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, property.PropertyType, FieldAttributes.Private);
            fieldBuilder.SetCustomAttribute(() => new CompilerGeneratedAttribute());
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, Type.EmptyTypes);
            if (property.CanRead)
            {
                MethodBuilder propertyGet = typeBuilder.DefineMethod("get_" + property.Name, methodAttributes, property.PropertyType, Type.EmptyTypes);
                propertyGet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                ILGenerator iLGenerator = propertyGet.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                iLGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(propertyGet);
            }

            if (property.CanWrite)
            {
                MethodBuilder propertySet = typeBuilder.DefineMethod("set_" + property.Name, methodAttributes, typeof(void), new Type[] { property.PropertyType });
                propertySet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                propertySet.DefineParameter(1, ParameterAttributes.None, "value");
                ILGenerator iLGenerator = propertySet.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                iLGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetSetMethod(propertySet);
            }

            propertyBuilder.CopyCustomAttributes(property);
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
