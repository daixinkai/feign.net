using Feign.Internal;
using Feign.Proxy;
using Feign.Request;
using Feign.Request.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    /// <summary>
    /// 默认的方法生成器
    /// </summary>
    internal class FeignClientHttpProxyEmitMethodBuilder : IMethodBuilder
    {
        #region define
        protected static MethodInfo GetReplacePathVariableMethod(TypeBuilder typeBuilder, Type valueType)
        {
            if (valueType == typeof(string))
            {
                return typeBuilder.BaseType!
                .GetRequiredMethod("ReplaceStringPathVariable", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            if (valueType.IsPrimitive)
            {
                return typeBuilder.BaseType!
                    .GetRequiredMethod("ReplaceToStringPathVariable", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(valueType)
                    ;
            }
            if (valueType.IsNullableType() && valueType.GenericTypeArguments[0].IsPrimitive)
            {
                return typeBuilder.BaseType!
                    .GetRequiredMethod("ReplaceNullablePathVariable", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(valueType.GenericTypeArguments[0])
                    ;
            }
            return typeBuilder.BaseType!
                .GetRequiredMethod("ReplacePathVariable", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType)
                ;
        }

        protected static MethodInfo GetReplaceRequestQueryMethod(TypeBuilder typeBuilder, Type valueType)
        {
            if (valueType == typeof(string))
            {
                return typeBuilder.BaseType!
                .GetRequiredMethod("ReplaceStringRequestQuery", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            if (valueType.IsPrimitive)
            {
                return typeBuilder.BaseType!
                    .GetRequiredMethod("ReplaceToStringRequestQuery", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(valueType)
                    ;
            }
            if (valueType.IsNullableType() && valueType.GenericTypeArguments[0].IsPrimitive)
            {
                return typeBuilder.BaseType!
                    .GetRequiredMethod("ReplaceNullableRequestQuery", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(valueType.GenericTypeArguments[0])
                    ;
            }
            return typeBuilder.BaseType!
                .GetRequiredMethod("ReplaceRequestQuery", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType)
                ;
        }
        protected static MethodInfo GetConvertToStringValueMethod(TypeBuilder typeBuilder, Type valueType)
        {
            var toStringMethod = StringValueMethods.GetToStringMethod(valueType);
            if (toStringMethod != null)
            {
                return toStringMethod;
            }
            return typeBuilder.BaseType!
                .GetRequiredMethod("ConvertToStringValue", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType)
                ;
        }
        #endregion

        public FeignClientMethodInfo BuildMethod(TypeBuilder typeBuilder, Type serviceType, MethodInfo method, FeignClientAttribute feignClientAttribute)
        {
            return BuildMethod(typeBuilder, serviceType, method, feignClientAttribute, GetRequestMappingAttribute(method));
        }

        private FeignClientMethodInfo BuildMethod(TypeBuilder typeBuilder, Type serviceType, MethodInfo method, FeignClientAttribute feignClientAttribute, RequestMappingBaseAttribute? requestMapping)
        {
            FeignClientMethodInfo feignClientMethodInfo = new()
            {
                MethodId = GetMethodId(method),
                MethodMetadata = method
            };
            //创建方法
            MethodBuilder methodBuilder = CreateMethodBuilder(typeBuilder, method);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            if (requestMapping == null)
            {
                //如果找不到mapping,抛出 NotSupportedException 异常
                iLGenerator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetEmptyConstructor()!);
                iLGenerator.Emit(OpCodes.Throw);
                return new FeignClientMethodInfo
                {
                    MethodId = GetMethodId(method),
                    MethodMetadata = method
                };
            }
            string uri = requestMapping.Value ?? "";
            LocalBuilder local_Uri = iLGenerator.DeclareLocal(typeof(string)); // 定义uri 
            iLGenerator.Emit(OpCodes.Ldstr, uri);
            iLGenerator.Emit(OpCodes.Stloc, local_Uri);
            List<EmitRequestContent> emitRequestContents = EmitParameter(typeBuilder, requestMapping, iLGenerator, method, local_Uri);
            EmitCallMethod(typeBuilder, methodBuilder, iLGenerator, serviceType, feignClientMethodInfo, feignClientAttribute, requestMapping, local_Uri, emitRequestContents);
            methodBuilder.CopyCustomAttributes(method);
            return feignClientMethodInfo;
        }

        /// <summary>
        /// <para>获取调用的Send方法 </para>
        /// <para><see cref="Proxy.FeignClientHttpProxy{TService}.SendAsync(FeignClientHttpRequest)"/></para>
        /// <para><see cref="Proxy.FeignClientHttpProxy{TService}.SendAsync{TResult}(FeignClientHttpRequest)"/></para>
        /// <para><see cref="Proxy.FeignClientHttpProxy{TService}.Send(FeignClientHttpRequest)"/></para>
        /// <para><see cref="Proxy.FeignClientHttpProxy{TService}.Send{TResult}(FeignClientHttpRequest)"/></para>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="method"></param>
        /// <param name="requestMapping"></param>
        /// <returns></returns>
        protected MethodInfo GetInvokeMethod(Type serviceType, MethodInfo method, RequestMappingBaseAttribute requestMapping)
        {
            return GetInvokeMethod(serviceType, requestMapping, GetReturnType(method), method.IsTaskMethod() || method.IsValueTaskMethod());
        }

        private static Type GetReturnType(MethodInfo method)
        {
            Type returnType;
            if ((method.IsTaskMethod() || method.IsValueTaskMethod()) && method.ReturnType.IsGenericType)
            {
                returnType = method.ReturnType.GenericTypeArguments[0];
            }
            else
            {
                returnType = method.ReturnType;
            }
            return returnType;
        }

        protected virtual MethodInfo GetInvokeMethod(Type serviceType, RequestMappingBaseAttribute requestMapping, Type? returnType, bool async)
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
                    FeignClientHttpProxy<object>.GetHttpSendAsyncGenericMethod(serviceType)
                    : FeignClientHttpProxy<object>.GetHttpSendGenericMethod(serviceType);
            }
            else
            {
                httpClientMethod = async ?
                    FeignClientHttpProxy<object>.GetHttpSendAsyncMethod(serviceType)
                    : FeignClientHttpProxy<object>.GetHttpSendMethod(serviceType);
            }
            if (isGeneric)
            {
                return httpClientMethod.MakeGenericMethod(returnType!);
            }
            return httpClientMethod;
        }

        protected static RequestMappingBaseAttribute? GetRequestMappingAttribute(MethodInfo method)
        {
            if (method.IsDefined(typeof(RequestMappingBaseAttribute)))
            {
                RequestMappingBaseAttribute[] requestMappingBaseAttributes = method.GetCustomAttributes<RequestMappingBaseAttribute>().ToArray();
                if (requestMappingBaseAttributes.Length > 1)
                {
                    throw new ArgumentException(nameof(requestMappingBaseAttributes.Length));
                }
                return requestMappingBaseAttributes[0];
            }
            string methodName = method.Name.ToLower();

            if (methodName.StartsWith("get") || methodName.StartsWith("query") || methodName.StartsWith("select"))
            {
                //get
                return new GetMappingAttribute();
            }
            else if (methodName.StartsWith("post") || methodName.StartsWith("create") || methodName.StartsWith("insert"))
            {
                //post
                return new PostMappingAttribute();
            }
            else if (methodName.StartsWith("put") || methodName.StartsWith("update"))
            {
                //put
                return new PutMappingAttribute();
            }
            else if (methodName.StartsWith("delete") || methodName.StartsWith("remove"))
            {
                //delete
                return new DeleteMappingAttribute();
            }
            return null;
        }


        protected static MethodBuilder CreateMethodBuilder(TypeBuilder typeBuilder, MethodInfo method)
        {
            MethodAttributes methodAttributes;
            if (method.IsVirtual)
            {
                //methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
                methodAttributes =
                    MethodAttributes.Public
                    | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot
                    | MethodAttributes.Virtual
                    | MethodAttributes.Final;
            }
            else
            {
                methodAttributes = MethodAttributes.Public;
            }
            MethodBuilder methodBuilder = typeBuilder.DefineMethodBuilder(method, methodAttributes, true);
            typeBuilder.DefineMethodOverride(methodBuilder, method);
            return methodBuilder;
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="methodBuilder"></param>
        /// <param name="iLGenerator"></param>
        /// <param name="serviceType"></param>
        /// <param name="feignClientMethodInfo"></param>
        /// <param name="feignClientAttribute"></param>
        /// <param name="requestMapping"></param>
        /// <param name="uri"></param>
        /// <param name="emitRequestContents"></param>
        protected virtual void EmitCallMethod(TypeBuilder typeBuilder, MethodBuilder methodBuilder, ILGenerator iLGenerator, Type serviceType, FeignClientMethodInfo feignClientMethodInfo, FeignClientAttribute feignClientAttribute, RequestMappingBaseAttribute requestMapping, LocalBuilder uri, List<EmitRequestContent>? emitRequestContents)
        {
            //得到Send方法
            var invokeMethod = GetInvokeMethod(serviceType, feignClientMethodInfo.MethodMetadata!, requestMapping);
            if (emitRequestContents != null && emitRequestContents.Count > 0 && !requestMapping.IsSupportRequestContent())
            {
                throw new NotSupportedException("RequestBody or RequestForm is not supported");
            }
            //定义请求的详情 FeignClientHttpRequest
            LocalBuilder feignClientRequest = DefineFeignClientRequest(typeBuilder, serviceType, iLGenerator, uri, requestMapping, emitRequestContents, feignClientMethodInfo, feignClientAttribute);
            iLGenerator.Emit(OpCodes.Ldarg_0);  //this
            iLGenerator.Emit(OpCodes.Ldloc, feignClientRequest);
            iLGenerator.Emit(OpCodes.Call, invokeMethod);
#if USE_VALUE_TASK
            if (feignClientMethodInfo.MethodMetadata!.IsValueTaskMethod())
            {
                EmitTaskToValueTask(iLGenerator, feignClientMethodInfo.MethodMetadata!.ReturnType);
            }
#endif
            iLGenerator.Emit(OpCodes.Ret);
        }
#if USE_VALUE_TASK
        /// <summary>
        /// new ValueTask(task)
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="returnType"></param>
        protected void EmitTaskToValueTask(ILGenerator iLGenerator, Type returnType)
        {
            ConstructorInfo constructorInfo;
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var resultType = returnType.GetGenericArguments()[0];
                constructorInfo = typeof(ValueTask<>).MakeGenericType(resultType).GetConstructor(new Type[] { typeof(Task<>).MakeGenericType(resultType) })!;
            }
            else
            {
                constructorInfo = typeof(ValueTask).GetConstructor(new Type[] { typeof(Task) })!;
            }
            iLGenerator.Emit(OpCodes.Newobj, constructorInfo);
        }
#endif
        /// <summary>
        /// 定义用于Send方法的FeignClientHttpRequest
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="serviceType"></param>
        /// <param name="iLGenerator"></param>
        /// <param name="uri"></param>
        /// <param name="requestMapping"></param>
        /// <param name="emitRequestContents"></param>
        /// <param name="feignClientMethodInfo"></param>
        /// <param name="feignClientAttribute"></param>
        /// <returns></returns>
        protected LocalBuilder DefineFeignClientRequest(TypeBuilder typeBuilder, Type serviceType, ILGenerator iLGenerator, LocalBuilder uri, RequestMappingBaseAttribute requestMapping, List<EmitRequestContent>? emitRequestContents, FeignClientMethodInfo feignClientMethodInfo, FeignClientAttribute feignClientAttribute)
        {
            Type returnType = GetReturnType(feignClientMethodInfo.MethodMetadata!);
            var feignClientMethodInfoLocalBuilder = DefineFeignClientMethodInfo(typeBuilder, iLGenerator, feignClientMethodInfo, returnType);

            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(FeignClientHttpRequest));

            #region localBuilder = new FeignClientHttpRequest(string baseUrl, string mappingUri, string uri, string httpMethod, string contentType)

            // baseUrl
            iLGenerator.Emit(OpCodes.Ldarg_0); //this
            iLGenerator.Emit(OpCodes.Callvirt, typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType).GetRequiredProperty("BaseUrl", BindingFlags.Instance | BindingFlags.NonPublic).GetMethod!);

            //mapping uri
            if (requestMapping.Value == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, requestMapping.Value);
            }
            //uri
            iLGenerator.Emit(OpCodes.Ldloc, uri);
            //httpMethod
            iLGenerator.Emit(OpCodes.Ldstr, requestMapping.GetMethod());

            //contentType
            string? contentType = requestMapping.ContentType;
            if (string.IsNullOrWhiteSpace(contentType) && serviceType.IsDefined(typeof(RequestMappingAttribute)))
            {
                contentType = serviceType.GetCustomAttribute<RequestMappingAttribute>()!.ContentType;
            }
            if (contentType == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, contentType);
            }

            //new FeignClientHttpRequest(baseUrl, mappingUri, uri, httpMethod, contentType)
            iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpRequest).GetFirstConstructor());
            #region HttpCompletionOption
            if (requestMapping.CompletionOption != default)
            {
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.EmitEnumValue(requestMapping.CompletionOption);
                iLGenerator.Emit(OpCodes.Callvirt, typeof(FeignClientHttpRequest).GetRequiredProperty("CompletionOption").SetMethod!);
            }
            #endregion


            #endregion

            #region FeignClientHttpRequest.Method

            iLGenerator.Emit(OpCodes.Dup);
            iLGenerator.Emit(OpCodes.Ldloc, feignClientMethodInfoLocalBuilder);
            iLGenerator.EmitSetProperty(typeof(FeignClientHttpRequest).GetRequiredProperty("Method"));
            #endregion

            #region FeignClientHttpRequest.Accept
            //accept
            string? accept = requestMapping.Accept;
            if (string.IsNullOrWhiteSpace(accept) && serviceType.IsDefined(typeof(RequestMappingAttribute)))
            {
                accept = serviceType.GetCustomAttribute<RequestMappingAttribute>()!.Accept;
            }
            if (accept != null)
            {
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.Emit(OpCodes.Ldstr, accept);
                iLGenerator.EmitSetProperty(typeof(FeignClientHttpRequest).GetRequiredProperty("Accept"));
            }

            #endregion

            #region FeignClientHttpRequest.Headers
            //headers
            if (feignClientMethodInfo.MethodMetadata!.IsDefined(typeof(HeadersAttribute), true))
            {
                List<IEmitValue<string>> headers = new();
                var methodHeaders = feignClientMethodInfo.MethodMetadata.GetCustomAttribute<HeadersAttribute>()!.Headers;
                if (methodHeaders != null)
                {
                    headers.AddRange(methodHeaders.Select(s => new EmitConstantStringValue(s)));
                }
                if (headers.Count > 0)
                {
                    iLGenerator.Emit(OpCodes.Dup);
                    iLGenerator.EmitStringArray(headers);
                    iLGenerator.EmitSetProperty(typeof(FeignClientHttpRequest).GetRequiredProperty("Headers"));
                }
            }

            #endregion

            #region FeignClientHttpRequest.Dismiss404
            //Dismiss404
            if (feignClientAttribute.Dismiss404)
            {
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.Emit(OpCodes.Ldc_I4_1);
                iLGenerator.EmitSetProperty(typeof(FeignClientHttpRequest).GetRequiredProperty("Dismiss404"));
            }
            #endregion

            #region FeignClientHttpRequest.IsSpecialResult
            if (SpecialResults.IsSpecialResult(returnType))
            {
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.Emit(OpCodes.Ldc_I4_1);
                iLGenerator.EmitSetProperty(typeof(FeignClientHttpRequest).GetRequiredProperty("IsSpecialResult"));
            }
            #endregion

            iLGenerator.Emit(OpCodes.Stloc, localBuilder);

            #region FeignClientHttpRequest.RequestContent
            //requestContent
            if (emitRequestContents != null && emitRequestContents.Count > 0)
            {
                iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                if (emitRequestContents.Count == 1)
                {
                    if (typeof(IHttpRequestFile).IsAssignableFrom(emitRequestContents[0].Parameter.ParameterType))
                    {
                        EmitFeignClientMultipartRequestContent(iLGenerator, emitRequestContents);
                    }
                    else
                    {
                        EmitFeignClientRequestContent(iLGenerator, emitRequestContents[0], null);
                    }
                }
                else if (emitRequestContents.Any(s => !s.SupportMultipart))
                {
                    throw new NotSupportedException("最多只支持一个RequestContent \r\n " + feignClientMethodInfo.ToString());
                }
                else
                {
                    EmitFeignClientMultipartRequestContent(iLGenerator, emitRequestContents);
                }
                iLGenerator.EmitSetProperty(typeof(FeignClientHttpRequest).GetRequiredProperty("RequestContent"));
            }
            #endregion

            #region FeignClientHttpRequest.RequestHeaderHandlers
            // request headers
            List<Tuple<int, ParameterInfo, RequestHeaderBaseAttribute>> headerBaseAttributes = new();
            int parameterIndex = -1;
            foreach (var item in feignClientMethodInfo.MethodMetadata.GetParameters())
            {
                parameterIndex++;
                if (!item.IsDefined(typeof(RequestHeaderBaseAttribute)))
                {
                    continue;
                }
                headerBaseAttributes.Add(Tuple.Create(parameterIndex, item, item.GetCustomAttribute<RequestHeaderBaseAttribute>()!));
            }
            if (headerBaseAttributes.Count > 0)
            {
                //feignClientHttpRequest.RequestHeaderHandlers = new List<IRequestHeaderHandler>();
                iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                iLGenerator.Emit(OpCodes.Newobj, typeof(List<IRequestHeaderHandler>).GetEmptyConstructor()!);
                iLGenerator.Emit(OpCodes.Callvirt, typeof(FeignClientHttpRequest).GetRequiredProperty("RequestHeaderHandlers").SetMethod!);

                MethodInfo listAddItemMethodInfo = typeof(List<IRequestHeaderHandler>).GetRequiredMethod("Add");
                foreach (var headerBaseAttribute in headerBaseAttributes)
                {
                    //string text2 = "xxx";
                    //feignClientHttpRequest.RequestHeaderHandlers.Add(new RequestHeaderHandler("xxx", text2));
                    var valueBuilder = iLGenerator.DeclareLocal(typeof(string));
                    iLGenerator.EmitLdarg(headerBaseAttribute.Item1 + 1);
                    if (headerBaseAttribute.Item2.ParameterType != typeof(string))
                    {
                        iLGenerator.Emit(OpCodes.Call, GetConvertToStringValueMethod(typeBuilder, headerBaseAttribute.Item2.ParameterType));
                    }
                    iLGenerator.Emit(OpCodes.Stloc, valueBuilder);
                    var handlerLocalBuilder = headerBaseAttribute.Item3.EmitNewRequestHeaderHandler(iLGenerator, valueBuilder);
                    if (handlerLocalBuilder != null)
                    {
                        iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                        iLGenerator.Emit(OpCodes.Callvirt, typeof(FeignClientHttpRequest).GetRequiredProperty("RequestHeaderHandlers").GetMethod!);
                        iLGenerator.Emit(OpCodes.Ldloc, handlerLocalBuilder);
                        iLGenerator.Emit(OpCodes.Callvirt, listAddItemMethodInfo);
                    }
                }
            }
            #endregion


            return localBuilder;
        }

        private static LocalBuilder DefineFeignClientMethodInfo(TypeBuilder typeBuilder, ILGenerator iLGenerator, FeignClientMethodInfo feignClientMethodInfo, Type returnType)
        {
            //feignClientMethodInfo
            //feignClientMethodInfo=null
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(FeignClientMethodInfo));
            iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientMethodInfo).GetEmptyConstructor()!);
            iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
            iLGenerator.Emit(OpCodes.Ldstr, feignClientMethodInfo.MethodId!);
            iLGenerator.EmitSetProperty(typeof(FeignClientMethodInfo).GetRequiredProperty("MethodId"));
            #region ResultType
            //feignClientMethodInfo.ResultType=typeof(xx);
            ResultTypeAttribute? resultTypeAttribute = feignClientMethodInfo.MethodMetadata!.GetCustomAttribute<ResultTypeAttribute>();
            if (returnType != null && resultTypeAttribute != null)
            {
                Type? resultType = resultTypeAttribute.ConvertType(returnType);
                if (resultType != null)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                    iLGenerator.EmitType(resultType);
                    iLGenerator.EmitSetProperty(typeof(FeignClientMethodInfo).GetRequiredProperty("ResultType"));
                }
                feignClientMethodInfo.ResultType = resultType;
            }
            #endregion

            Label nextLabel = iLGenerator.DefineLabel();

            #region if (base.FeignOptions.IncludeMethodMetadata) set the call method
            //这里获取方法元数据
            PropertyInfo feignOptionsProperty = typeBuilder.BaseType!.GetRequiredProperty("FeignOptions", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo includeMethodMetadataProperty = feignOptionsProperty.PropertyType.GetRequiredProperty("IncludeMethodMetadata");
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.EmitGetProperty(feignOptionsProperty);
            iLGenerator.Emit(OpCodes.Callvirt, includeMethodMetadataProperty.GetMethod!);
            iLGenerator.Emit(OpCodes.Ldc_I4_1);
            iLGenerator.Emit(OpCodes.Ceq);
            iLGenerator.Emit(OpCodes.Brfalse_S, nextLabel);
            iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
            iLGenerator.EmitMethodInfo(feignClientMethodInfo.MethodMetadata!);
            iLGenerator.EmitSetProperty(typeof(FeignClientMethodInfo).GetRequiredProperty("MethodMetadata"));
            #endregion
            //处理下 if GOTO
            iLGenerator.MarkLabel(nextLabel);
            return localBuilder;
        }

        private string? GetMethodId(MethodInfo methodInfo)
        {
            if (methodInfo.IsDefined(typeof(MethodIdAttribute)))
            {
                return methodInfo.GetCustomAttribute<MethodIdAttribute>()!.MethodId;
            }
            if (methodInfo.DeclaringType!.GetMethods().Where(s => s.Name == methodInfo.Name).Count() == 1)
            {
                return methodInfo.Name;
            }
            return methodInfo.Name + "(" + string.Join(",", methodInfo.GetParameters().Select(s => GetTypeName(s.ParameterType))) + ")";
        }

        private string GetTypeName(Type type)
        {
            if (type.IsNullableType())
            {
                return GetTypeName(type.GenericTypeArguments[0]) + "?";
            }
            if (type.IsPrimitive || type == typeof(string))
            {
                return type.Name;
            }
            return type.FullName!;
        }

        private static void EmitFeignClientRequestContent(ILGenerator iLGenerator, EmitRequestContent emitRequestContent, LocalBuilder? localBuilder)
        {
            if (typeof(IHttpRequestFileForm).IsAssignableFrom(emitRequestContent.Parameter.ParameterType))
            {
                //iLGenerator.Emit(OpCodes.Ldstr, emitRequestContent.Parameter.Name);
                iLGenerator.EmitLdarg(emitRequestContent.ParameterIndex);
                iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpFileFormRequestContent).GetFirstConstructor());
                if (localBuilder != null)
                {
                    iLGenerator.Emit(OpCodes.Stloc, localBuilder);
                }
                return;
            }
            if (typeof(IHttpRequestFile).IsAssignableFrom(emitRequestContent.Parameter.ParameterType))
            {
                iLGenerator.Emit(OpCodes.Ldstr, emitRequestContent.Parameter.Name!);
                iLGenerator.EmitLdarg(emitRequestContent.ParameterIndex);
                iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpFileRequestContent).GetFirstConstructor());
                if (localBuilder != null)
                {
                    iLGenerator.Emit(OpCodes.Stloc, localBuilder);
                }
                return;
            }
            ConstructorInfo constructorInfo;
            switch (emitRequestContent.MediaType)
            {
                case "application/json":
                    constructorInfo = typeof(FeignClientHttpJsonRequestContent<>).MakeGenericType(emitRequestContent.Parameter.ParameterType).GetFirstConstructor();
                    break;
                case "application/x-www-form-urlencoded":
                    constructorInfo = typeof(FeignClientHttpFormRequestContent<>).MakeGenericType(emitRequestContent.Parameter.ParameterType).GetFirstConstructor();
                    break;
                default:
                    throw new NotSupportedException($"ContentType {emitRequestContent.MediaType}  is not supported");
                    //constructorInfo = typeof(FeignClientFormRequestContent<>).MakeGenericType(emitRequestContent.Parameter.ParameterType).GetConstructors()[0];
                    //break;
            };
            iLGenerator.Emit(OpCodes.Ldstr, emitRequestContent.Parameter.Name!);
            iLGenerator.EmitLdarg(emitRequestContent.ParameterIndex);
            iLGenerator.Emit(OpCodes.Newobj, constructorInfo);
            if (localBuilder != null)
            {
                iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            }
        }

        private void EmitFeignClientMultipartRequestContent(ILGenerator iLGenerator, List<EmitRequestContent> emitRequestContents)
        {
            LocalBuilder requestContent = iLGenerator.DeclareLocal(typeof(FeignClientHttpMultipartFormRequestContent));
            MethodInfo methodAddContent = typeof(FeignClientHttpMultipartFormRequestContent).GetRequiredMethod("AddContent");
            iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpMultipartFormRequestContent).GetFirstConstructor());
            iLGenerator.Emit(OpCodes.Stloc, requestContent);
            for (int i = 0; i < emitRequestContents.Count; i++)
            {
                LocalBuilder childRequestContent = iLGenerator.DeclareLocal(typeof(FeignClientHttpRequestContent));
                EmitFeignClientRequestContent(iLGenerator, emitRequestContents[i], childRequestContent);
                iLGenerator.Emit(OpCodes.Ldloc, requestContent);
                iLGenerator.Emit(OpCodes.Ldstr, emitRequestContents[i].Parameter.Name!);
                iLGenerator.Emit(OpCodes.Ldloc, childRequestContent);
                iLGenerator.Emit(OpCodes.Call, methodAddContent);
            }
            iLGenerator.Emit(OpCodes.Ldloc, requestContent);
        }

        /// <summary>
        /// 处理参数
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="requestMapping"></param>
        /// <param name="iLGenerator"></param>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected static List<EmitRequestContent> EmitParameter(TypeBuilder typeBuilder, RequestMappingBaseAttribute requestMapping, ILGenerator iLGenerator, MethodInfo method, LocalBuilder uri)
        {
            int index = 0;
            List<EmitRequestContent> emitRequestContents = new();
            foreach (var parameterInfo in method.GetParameters())
            {
                index++;
                if (parameterInfo.GetCustomAttributes().Any(s => s is INotRequestParameter))
                {
                    continue;
                }
                if (typeof(IHttpRequestFileForm).IsAssignableFrom(parameterInfo.ParameterType))
                {
                    emitRequestContents.Add(new EmitRequestContent
                    {
                        MediaType = Constants.MediaTypes.MULTIPART_FORMDATA,
                        Parameter = parameterInfo,
                        SupportMultipart = false,
                        ParameterIndex = index
                    });
                    continue;
                }
                if (typeof(IHttpRequestFile).IsAssignableFrom(parameterInfo.ParameterType))
                {
                    emitRequestContents.Add(new EmitRequestContent
                    {
                        MediaType = Constants.MediaTypes.FORMDATA,
                        Parameter = parameterInfo,
                        SupportMultipart = true,
                        ParameterIndex = index
                    });
                    continue;
                }
                if (parameterInfo.IsDefined(typeof(RequestBodyAttribute)))
                {
                    emitRequestContents.Add(new EmitRequestContent
                    {
                        MediaType = Constants.MediaTypes.APPLICATION_JSON,
                        Parameter = parameterInfo,
                        SupportMultipart = false,
                        ParameterIndex = index
                    });
                    continue;
                }
                if (parameterInfo.IsDefined(typeof(RequestFormAttribute)))
                {
                    emitRequestContents.Add(new EmitRequestContent
                    {
                        MediaType = Constants.MediaTypes.APPLICATION_FORM_URLENCODED,
                        Parameter = parameterInfo,
                        SupportMultipart = true,
                        ParameterIndex = index
                    });
                    continue;
                }
                MethodInfo replaceValueMethod;
                string? name;
                if (parameterInfo.IsDefined(typeof(RequestParamAttribute)))
                {
                    //如果是 HttpGet , 拼接到url上
                    if (requestMapping.IsHttpMethod(HttpMethod.Get) || requestMapping.IsHttpMethod(HttpMethod.Head))
                    {
                        name = parameterInfo.GetCustomAttribute<RequestParamAttribute>()!.Name ?? parameterInfo.Name;
                        //replaceValueMethod = ReplaceRequestParamMethod;
                        replaceValueMethod = GetReplaceRequestQueryMethod(typeBuilder, parameterInfo.ParameterType);
                    }
                    else
                    {
                        emitRequestContents.Add(new EmitRequestContent
                        {
                            MediaType = Constants.MediaTypes.APPLICATION_FORM_URLENCODED,
                            Parameter = parameterInfo,
                            SupportMultipart = true,
                            ParameterIndex = index
                        });
                        continue;
                    }
                }
                else if (parameterInfo.IsDefined(typeof(RequestQueryAttribute)))
                {
                    name = parameterInfo.Name;
                    //replaceValueMethod = ReplaceRequestQueryMethod;
                    replaceValueMethod = GetReplaceRequestQueryMethod(typeBuilder, parameterInfo.ParameterType);
                }
                else if (parameterInfo.IsDefined(typeof(PathVariableAttribute)))
                {
                    name = parameterInfo.GetCustomAttribute<PathVariableAttribute>()!.Name;
                    //replaceValueMethod = ReplacePathVariableMethod;
                    replaceValueMethod = GetReplacePathVariableMethod(typeBuilder, parameterInfo.ParameterType);
                }
                else
                {
                    name = parameterInfo.Name;
                    if (FeignClientUtils.ContainsPathVariable(requestMapping.Value, name))
                    {
                        //replaceValueMethod = ReplacePathVariableMethod;
                        replaceValueMethod = GetReplacePathVariableMethod(typeBuilder, parameterInfo.ParameterType);
                    }
                    else
                    {
                        //replaceValueMethod = ReplaceRequestQueryMethod;
                        replaceValueMethod = GetReplaceRequestQueryMethod(typeBuilder, parameterInfo.ParameterType);
                    }
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = parameterInfo.Name;
                }


                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldloc, uri);
                iLGenerator.Emit(OpCodes.Ldstr, name!);
                iLGenerator.EmitLdarg(index);
                iLGenerator.Emit(OpCodes.Call, replaceValueMethod);
                iLGenerator.Emit(OpCodes.Stloc, uri);

            }
            return emitRequestContents;
        }

    }
}
