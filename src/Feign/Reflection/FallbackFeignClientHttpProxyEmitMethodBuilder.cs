using Feign.Internal;
using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    /// <summary>
    /// 支持降级服务的方法生成器
    /// </summary>
    internal class FallbackFeignClientHttpProxyEmitMethodBuilder : FeignClientHttpProxyEmitMethodBuilder
    {
        public FallbackFeignClientHttpProxyEmitMethodBuilder(DynamicAssembly dynamicAssembly)
        {
            _dynamicAssembly = dynamicAssembly;
            _fallbackProxyAnonymousMethodClassBuilder = new FallbackProxyAnonymousMethodClassBuilder();
        }

        private readonly DynamicAssembly _dynamicAssembly;

        private readonly FallbackProxyAnonymousMethodClassBuilder _fallbackProxyAnonymousMethodClassBuilder;


        protected override MethodInfo GetInvokeMethod(Type serviceType, RequestMappingBaseAttribute requestMapping, Type? returnType, bool async)
        {
            MethodInfo httpClientMethod;
            bool isGeneric = returnType != null && returnType != typeof(void) && returnType != typeof(Task)
#if USE_VALUE_TASK
                && returnType != typeof(ValueTask)
#endif
                ;
            if (isGeneric)
            {
                httpClientMethod = async ?
                    FallbackFeignClientHttpProxy<object, object>.GetHttpSendAsyncGenericFallbackMethod(serviceType, serviceType)
                    : FallbackFeignClientHttpProxy<object, object>.GetHttpSendGenericFallbackMethod(serviceType, serviceType);
            }
            else
            {
                httpClientMethod = async ?
                    FallbackFeignClientHttpProxy<object, object>.GetHttpSendAsyncFallbackMethod(serviceType, serviceType)
                    : FallbackFeignClientHttpProxy<object, object>.GetHttpSendFallbackMethod(serviceType, serviceType);
            }
            if (isGeneric)
            {
                return httpClientMethod.MakeGenericMethod(returnType!);
            }
            return httpClientMethod;
        }

        protected override void EmitCallMethod(TypeBuilder typeBuilder, MethodBuilder methodBuilder, ILGenerator iLGenerator, Type serviceType, FeignClientMethodInfo feignClientMethodInfo, RequestMappingBaseAttribute requestMapping, LocalBuilder uri, List<EmitRequestContent>? emitRequestContents)
        {
            var invokeMethod = GetInvokeMethod(serviceType, feignClientMethodInfo.MethodMetadata!, requestMapping);
            if (emitRequestContents != null && emitRequestContents.Count > 0 && !requestMapping.IsSupportRequestContent())
            {
                throw new NotSupportedException("RequestBody or RequestForm is not supported");
            }
            LocalBuilder feignClientRequest = DefineFeignClientRequest(typeBuilder, serviceType, iLGenerator, uri, requestMapping, emitRequestContents, feignClientMethodInfo);
            // fallback
            LocalBuilder fallbackDelegate = DefineFallbackDelegate(typeBuilder, iLGenerator, serviceType, feignClientMethodInfo.MethodMetadata!);
            iLGenerator.Emit(OpCodes.Ldarg_0);  //this
            iLGenerator.Emit(OpCodes.Ldloc, feignClientRequest);
            iLGenerator.Emit(OpCodes.Ldloc, fallbackDelegate);
            iLGenerator.Emit(OpCodes.Call, invokeMethod);
#if USE_VALUE_TASK
            if (feignClientMethodInfo.MethodMetadata!.IsValueTaskMethod())
            {
                EmitTaskToValueTask(iLGenerator, feignClientMethodInfo.MethodMetadata!.ReturnType, feignClientMethodInfo.ResultType);
            }
#endif
            iLGenerator.Emit(OpCodes.Ret);
        }
        /// <summary>
        /// Generate fallback method delegate
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="iLGenerator"></param>
        /// <param name="serviceType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private LocalBuilder DefineFallbackDelegate(TypeBuilder typeBuilder, ILGenerator iLGenerator, Type serviceType, MethodInfo method)
        {
            Type delegateType;
            if (method.ReturnType == null || method.ReturnType == typeof(void))
            {
                delegateType = typeof(Action);
            }
            else
            {
                delegateType = typeof(Func<>).MakeGenericType(method.ReturnType);
            }

            int bindingFlagsValue = 0;
            foreach (var item in Enum.GetValues(typeof(BindingFlags)))
            {
                bindingFlagsValue += item!.GetHashCode();
            }
            var delegateConstructor = delegateType.GetConstructors((BindingFlags)bindingFlagsValue)[0];
            LocalBuilder invokeDelegate = iLGenerator.DeclareLocal(delegateType);
            // if has parameters
            if (method.GetParameters().Length > 0)
            {
                //方法含有参数的情况下,需要生成一个匿名代理类型
                var anonymousMethodClassTypeBuild = _fallbackProxyAnonymousMethodClassBuilder.BuildType(_dynamicAssembly.ModuleBuilder, serviceType, method);
                // new anonymousMethodClass
                LocalBuilder anonymousMethodClass = iLGenerator.DeclareLocal(anonymousMethodClassTypeBuild.Item1);
                //field
                iLGenerator.Emit(OpCodes.Ldarg_0); //this

                iLGenerator.EmitGetProperty(typeBuilder.BaseType!.GetRequiredProperty("Fallback")); //.Fallback

                iLGenerator.EmitLdargEx(1, method.GetParameters().Length);
                iLGenerator.Emit(OpCodes.Newobj, anonymousMethodClassTypeBuild.Item2);
                iLGenerator.Emit(OpCodes.Stloc, anonymousMethodClass);
                iLGenerator.Emit(OpCodes.Ldloc, anonymousMethodClass);
                iLGenerator.Emit(OpCodes.Ldftn, anonymousMethodClassTypeBuild.Item3);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldarg_0); //this
                iLGenerator.EmitGetProperty(typeBuilder.BaseType!.GetRequiredProperty("Fallback")); //.Fallback
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.Emit(OpCodes.Ldvirtftn, method);
            }

            iLGenerator.Emit(OpCodes.Newobj, delegateConstructor);
            iLGenerator.Emit(OpCodes.Stloc, invokeDelegate);

            return invokeDelegate;
        }


    }

}