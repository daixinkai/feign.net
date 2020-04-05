using Feign.Internal;
using Feign.Proxy;
using Feign.Request;
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
    class FeignClientHttpProxyEmitMethodBuilder : IMethodBuilder
    {
        #region define
        //protected static readonly MethodInfo ReplacePathVariableMethod = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(o => o.IsGenericMethod && o.Name == "ReplacePathVariable");

        //protected static readonly MethodInfo ReplaceRequestParamMethod = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(o => o.IsGenericMethod && o.Name == "ReplaceRequestParam");

        //protected static readonly MethodInfo ReplaceRequestQueryMethod = typeof(FeignClientHttpProxy<>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(o => o.IsGenericMethod && o.Name == "ReplaceRequestQuery");

        protected static MethodInfo GetReplacePathVariableMethod(TypeBuilder typeBuilder)
        {
            return typeBuilder.BaseType.GetMethod("ReplacePathVariable", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected static MethodInfo GetReplaceRequestParamMethod(TypeBuilder typeBuilder)
        {
            return typeBuilder.BaseType.GetMethod("ReplaceRequestParam", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected static MethodInfo GetReplaceRequestQueryMethod(TypeBuilder typeBuilder)
        {
            return typeBuilder.BaseType.GetMethod("ReplaceRequestQuery", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        #endregion

        public FeignClientMethodInfo BuildMethod(TypeBuilder typeBuilder, Type serviceType, MethodInfo method, FeignClientAttribute feignClientAttribute)
        {
            return BuildMethod(typeBuilder, serviceType, method, feignClientAttribute, GetRequestMappingAttribute(method));
        }

        FeignClientMethodInfo BuildMethod(TypeBuilder typeBuilder, Type serviceType, MethodInfo method, FeignClientAttribute feignClientAttribute, RequestMappingBaseAttribute requestMapping)
        {
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo
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
                iLGenerator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
                iLGenerator.Emit(OpCodes.Throw);
                return new FeignClientMethodInfo
                {
                    MethodId = GetMethodId(method),
                    MethodMetadata = method
                };
            }
            string uri = requestMapping.Value ?? "";
            LocalBuilder local_Uri = iLGenerator.DeclareLocal(typeof(string)); // 定义uri 
            LocalBuilder local_OldValue = iLGenerator.DeclareLocal(typeof(string)); // 定义temp,用来拼接请求的真实uri
            iLGenerator.Emit(OpCodes.Ldstr, uri);
            iLGenerator.Emit(OpCodes.Stloc, local_Uri);
            List<EmitRequestContent> emitRequestContents = EmitParameter(typeBuilder, requestMapping, iLGenerator, method, local_Uri, local_OldValue);
            EmitCallMethod(typeBuilder, methodBuilder, iLGenerator, serviceType, feignClientMethodInfo, requestMapping, local_Uri, emitRequestContents);
            methodBuilder.CopyCustomAttributes(method);
            return feignClientMethodInfo;
        }
        /// <summary>
        /// 获取调用的Send方法 
        /// <see cref="Proxy.FeignClientHttpProxy{TService}.SendAsync(FeignClientHttpRequest)"/>
        /// <see cref="Proxy.FeignClientHttpProxy{TService}.SendAsync{TResult}(FeignClientHttpRequest)"/>
        /// <see cref="Proxy.FeignClientHttpProxy{TService}.Send(FeignClientHttpRequest)"/>
        /// <see cref="Proxy.FeignClientHttpProxy{TService}.Send{TResult}(FeignClientHttpRequest)"/>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="method"></param>
        /// <param name="requestMapping"></param>
        /// <returns></returns>
        protected MethodInfo GetInvokeMethod(Type serviceType, MethodInfo method, RequestMappingBaseAttribute requestMapping)
        {
            return GetInvokeMethod(serviceType, requestMapping, GetReturnType(method), method.IsTaskMethod());
            //if (method.IsTaskMethod())
            //{
            //    if (method.ReturnType.IsGenericType)
            //    {
            //        return GetInvokeMethod(serviceType, requestMapping, method.ReturnType.GenericTypeArguments[0], true);
            //    }
            //    return GetInvokeMethod(serviceType, requestMapping, method.ReturnType, true);
            //}
            //return GetInvokeMethod(serviceType, requestMapping, method.ReturnType, false);
        }

        Type GetReturnType(MethodInfo method)
        {
            Type returnType;
            if (method.IsTaskMethod() && method.ReturnType.IsGenericType)
            {
                returnType = method.ReturnType.GenericTypeArguments[0];
            }
            else
            {
                returnType = method.ReturnType;
            }
            return returnType;
        }

        protected virtual MethodInfo GetInvokeMethod(Type serviceType, RequestMappingBaseAttribute requestMapping, Type returnType, bool async)
        {
            MethodInfo httpClientMethod;
            bool isGeneric = !(returnType == null || returnType == typeof(void) || returnType == typeof(Task));
            if (isGeneric)
            {
                //httpClientMethod = async ? FeignClientHttpProxy<object>.HTTP_SEND_ASYNC_GENERIC_METHOD : FeignClientHttpProxy<object>.HTTP_SEND_GENERIC_METHOD;
                httpClientMethod = async ? FeignClientHttpProxy<object>.GetHttpSendAsyncGenericMethod(serviceType) : FeignClientHttpProxy<object>.GetHttpSendGenericMethod(serviceType);
            }
            else
            {
                //  httpClientMethod = async ? FeignClientHttpProxy<object>.HTTP_SEND_ASYNC_METHOD : FeignClientHttpProxy<object>.HTTP_SEND_METHOD;
                httpClientMethod = async ? FeignClientHttpProxy<object>.GetHttpSendAsyncMethod(serviceType) : FeignClientHttpProxy<object>.GetHttpSendMethod(serviceType);
            }
            if (isGeneric)
            {
                return httpClientMethod.MakeGenericMethod(returnType);
            }
            return httpClientMethod;
        }

        /// <summary>
        /// 是否支持RequestContent
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestMappingBaseAttribute"></param>
        /// <returns></returns>
        protected bool SupportRequestContent(MethodInfo method, RequestMappingBaseAttribute requestMappingBaseAttribute)
        {
            return
                requestMappingBaseAttribute.IsHttpMethod(HttpMethod.Post) ||
                requestMappingBaseAttribute.IsHttpMethod(HttpMethod.Put) ||
                requestMappingBaseAttribute.IsHttpMethod(HttpMethod.Delete);
        }

        protected RequestMappingBaseAttribute GetRequestMappingAttribute(MethodInfo method)
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


        protected MethodBuilder CreateMethodBuilder(TypeBuilder typeBuilder, MethodInfo method)
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
            var arguments = method.GetParameters().Select(a => a.ParameterType).ToArray();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, arguments);
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
        /// <param name="requestMapping"></param>
        /// <param name="uri"></param>
        /// <param name="emitRequestContents"></param>
        protected virtual void EmitCallMethod(TypeBuilder typeBuilder, MethodBuilder methodBuilder, ILGenerator iLGenerator, Type serviceType, FeignClientMethodInfo feignClientMethodInfo, RequestMappingBaseAttribute requestMapping, LocalBuilder uri, List<EmitRequestContent> emitRequestContents)
        {
            //得到Send方法
            var invokeMethod = GetInvokeMethod(serviceType, feignClientMethodInfo.MethodMetadata, requestMapping);
            if (emitRequestContents != null && emitRequestContents.Count > 0 && !SupportRequestContent(invokeMethod, requestMapping))
            {
                throw new NotSupportedException("不支持RequestBody或者RequestForm");
            }
            //定义请求的详情 FeignClientHttpRequest
            LocalBuilder feignClientRequest = DefineFeignClientRequest(typeBuilder, serviceType, iLGenerator, uri, requestMapping, emitRequestContents, feignClientMethodInfo);
            iLGenerator.Emit(OpCodes.Ldarg_0);  //this
            iLGenerator.Emit(OpCodes.Ldloc, feignClientRequest);
            iLGenerator.Emit(OpCodes.Call, invokeMethod);
            iLGenerator.Emit(OpCodes.Ret);
        }
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
        /// <returns></returns>
        protected LocalBuilder DefineFeignClientRequest(TypeBuilder typeBuilder, Type serviceType, ILGenerator iLGenerator, LocalBuilder uri, RequestMappingBaseAttribute requestMapping, List<EmitRequestContent> emitRequestContents, FeignClientMethodInfo feignClientMethodInfo)
        {
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(FeignClientHttpRequest));
            // baseUrl
            EmitBaseUrl(iLGenerator, serviceType);
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
            string contentType = requestMapping.ContentType;
            if (string.IsNullOrWhiteSpace(contentType) && serviceType.IsDefined(typeof(RequestMappingAttribute)))
            {
                contentType = serviceType.GetCustomAttribute<RequestMappingAttribute>().ContentType;
            }
            if (contentType == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, contentType);
            }

            //accept
            string accept = requestMapping.Accept;
            if (string.IsNullOrWhiteSpace(contentType) && serviceType.IsDefined(typeof(RequestMappingAttribute)))
            {
                accept = serviceType.GetCustomAttribute<RequestMappingAttribute>().Accept;
            }
            if (accept == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, accept);
            }

            //headers
            List<string> headers = new List<string>();
            if (serviceType.IsDefined(typeof(HeadersAttribute), true))
            {
                var serviceHeaders = serviceType.GetCustomAttribute<HeadersAttribute>().Headers;
                if (serviceHeaders != null)
                {
                    headers.AddRange(serviceHeaders);
                }
            }
            if (feignClientMethodInfo.MethodMetadata.IsDefined(typeof(HeadersAttribute), true))
            {
                var methodHeaders = feignClientMethodInfo.MethodMetadata.GetCustomAttribute<HeadersAttribute>().Headers;
                if (methodHeaders != null)
                {
                    headers.AddRange(methodHeaders);
                }
            }
            if (headers.Count == 0)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.EmitStringArray(headers);
            }

            //requestContent
            if (emitRequestContents != null && emitRequestContents.Count > 0)
            {
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
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            #region new

            //feignClientMethodInfo
            //feignClientMethodInfo=null
            LocalBuilder feignClientMethodInfoLocalBuilder = iLGenerator.DeclareLocal(typeof(FeignClientMethodInfo));
            iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientMethodInfo).GetConstructors()[0]);
            iLGenerator.Emit(OpCodes.Stloc, feignClientMethodInfoLocalBuilder);
            iLGenerator.Emit(OpCodes.Ldloc, feignClientMethodInfoLocalBuilder);
            iLGenerator.Emit(OpCodes.Ldstr, feignClientMethodInfo.MethodId);
            iLGenerator.Emit(OpCodes.Call, typeof(FeignClientMethodInfo).GetProperty("MethodId").SetMethod);

            #region ResultType
            //feignClientMethodInfo.ResultType=typeof(xx);
            ResultTypeAttribute resultTypeAttribute = feignClientMethodInfo.MethodMetadata.GetCustomAttribute<ResultTypeAttribute>();
            Type returnType = GetReturnType(feignClientMethodInfo.MethodMetadata);
            if (returnType != null && resultTypeAttribute != null)
            {
                Type resultType = resultTypeAttribute.ConvertType(returnType);
                if (resultType != null)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, feignClientMethodInfoLocalBuilder);
                    iLGenerator.EmitType(resultType);
                    iLGenerator.Emit(OpCodes.Call, typeof(FeignClientMethodInfo).GetProperty("ResultType").SetMethod);
                }
            }
            #endregion


            Label newFeingClientRequestLabel = iLGenerator.DefineLabel();

            #region if (base.FeignOptions.IncludeMethodMetadata) set the call method
            //这里获取方法元数据
            PropertyInfo feignOptionsProperty = typeBuilder.BaseType.GetProperty("FeignOptions", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo includeMethodMetadataProperty = feignOptionsProperty.PropertyType.GetProperty("IncludeMethodMetadata");
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, feignOptionsProperty.GetMethod);
            iLGenerator.Emit(OpCodes.Callvirt, includeMethodMetadataProperty.GetMethod);
            iLGenerator.Emit(OpCodes.Ldc_I4, 1);
            iLGenerator.Emit(OpCodes.Ceq);
            iLGenerator.Emit(OpCodes.Brfalse_S, newFeingClientRequestLabel);
            iLGenerator.Emit(OpCodes.Ldloc, feignClientMethodInfoLocalBuilder);
            iLGenerator.EmitMethodInfo(feignClientMethodInfo.MethodMetadata);
            iLGenerator.Emit(OpCodes.Call, typeof(FeignClientMethodInfo).GetProperty("MethodMetadata").SetMethod);
            #endregion
            //处理下 if GOTO
            iLGenerator.MarkLabel(newFeingClientRequestLabel);

            iLGenerator.Emit(OpCodes.Ldloc, feignClientMethodInfoLocalBuilder);
            iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpRequest).GetConstructors()[0]);
            #region HttpCompletionOption
            //CompletionOption=requestMapping.CompletionOption.Value;
            iLGenerator.Emit(OpCodes.Dup);
            iLGenerator.EmitEnumValue(requestMapping.CompletionOption);
            iLGenerator.Emit(OpCodes.Callvirt, typeof(FeignClientHttpRequest).GetProperty("CompletionOption").SetMethod);
            #endregion
            iLGenerator.Emit(OpCodes.Stloc, localBuilder);

            return localBuilder;
            #endregion
        }

        string GetMethodId(MethodInfo methodInfo)
        {
            if (methodInfo.IsDefined(typeof(MethodIdAttribute)))
            {
                return methodInfo.GetCustomAttribute<MethodIdAttribute>().MethodId;
            }
            return methodInfo.Name + "(" + string.Join(",", methodInfo.GetParameters().Select(s => s.ParameterType.FullName)) + ")";
        }

        void EmitFeignClientRequestContent(ILGenerator iLGenerator, EmitRequestContent emitRequestContent, LocalBuilder localBuilder)
        {
            if (typeof(IHttpRequestFileForm).IsAssignableFrom(emitRequestContent.Parameter.ParameterType))
            {
                //iLGenerator.Emit(OpCodes.Ldstr, emitRequestContent.Parameter.Name);
                iLGenerator.Emit(OpCodes.Ldarg_S, emitRequestContent.ParameterIndex);
                iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpFileFormRequestContent).GetConstructors()[0]);
                if (localBuilder != null)
                {
                    iLGenerator.Emit(OpCodes.Stloc, localBuilder);
                }
                return;
            }
            if (typeof(IHttpRequestFile).IsAssignableFrom(emitRequestContent.Parameter.ParameterType))
            {
                iLGenerator.Emit(OpCodes.Ldstr, emitRequestContent.Parameter.Name);
                iLGenerator.Emit(OpCodes.Ldarg_S, emitRequestContent.ParameterIndex);
                iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpFileRequestContent).GetConstructors()[0]);
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
                    constructorInfo = typeof(FeignClientHttpJsonRequestContent<>).MakeGenericType(emitRequestContent.Parameter.ParameterType).GetConstructors()[0];
                    break;
                case "application/x-www-form-urlencoded":
                    constructorInfo = typeof(FeignClientHttpFormRequestContent<>).MakeGenericType(emitRequestContent.Parameter.ParameterType).GetConstructors()[0];
                    break;
                default:
                    throw new NotSupportedException("不支持的content type");
                    //constructorInfo = typeof(FeignClientFormRequestContent<>).MakeGenericType(emitRequestContent.Parameter.ParameterType).GetConstructors()[0];
                    //break;
            };
            iLGenerator.Emit(OpCodes.Ldstr, emitRequestContent.Parameter.Name);
            iLGenerator.Emit(OpCodes.Ldarg_S, emitRequestContent.ParameterIndex);
            iLGenerator.Emit(OpCodes.Newobj, constructorInfo);
            if (localBuilder != null)
            {
                iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            }
        }

        void EmitFeignClientMultipartRequestContent(ILGenerator iLGenerator, List<EmitRequestContent> emitRequestContents)
        {
            LocalBuilder requestContent = iLGenerator.DeclareLocal(typeof(FeignClientHttpMultipartFormRequestContent));
            MethodInfo methodAddContent = typeof(FeignClientHttpMultipartFormRequestContent).GetMethod("AddContent");
            iLGenerator.Emit(OpCodes.Newobj, typeof(FeignClientHttpMultipartFormRequestContent).GetConstructors()[0]);
            iLGenerator.Emit(OpCodes.Stloc, requestContent);
            for (int i = 0; i < emitRequestContents.Count; i++)
            {
                LocalBuilder childRequestContent = iLGenerator.DeclareLocal(typeof(FeignClientHttpRequestContent));
                EmitFeignClientRequestContent(iLGenerator, emitRequestContents[i], childRequestContent);
                iLGenerator.Emit(OpCodes.Ldloc, requestContent);
                iLGenerator.Emit(OpCodes.Ldstr, emitRequestContents[i].Parameter.Name);
                iLGenerator.Emit(OpCodes.Ldloc, childRequestContent);
                iLGenerator.Emit(OpCodes.Call, methodAddContent);
            }
            iLGenerator.Emit(OpCodes.Ldloc, requestContent);
        }

        void EmitBaseUrl(ILGenerator iLGenerator, Type serviceType)
        {
            PropertyInfo propertyInfo = typeof(FeignClientHttpProxy<>).MakeGenericType(serviceType).GetProperty("BaseUrl", BindingFlags.Instance | BindingFlags.NonPublic);
            iLGenerator.Emit(OpCodes.Ldarg_0); //this
            iLGenerator.Emit(OpCodes.Callvirt, propertyInfo.GetMethod);
        }

        /// <summary>
        /// 处理参数
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="requestMapping"></param>
        /// <param name="iLGenerator"></param>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected List<EmitRequestContent> EmitParameter(TypeBuilder typeBuilder, RequestMappingBaseAttribute requestMapping, ILGenerator iLGenerator, MethodInfo method, LocalBuilder uri, LocalBuilder value)
        {
            int index = 0;
            List<EmitRequestContent> emitRequestContents = new List<EmitRequestContent>();
            foreach (var parameterInfo in method.GetParameters())
            {
                index++;
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
                string name;
                if (parameterInfo.IsDefined(typeof(RequestParamAttribute)))
                {
                    //如果是 HttpGet , 拼接到url上
                    if (requestMapping.IsHttpMethod(HttpMethod.Get) || requestMapping.IsHttpMethod(HttpMethod.Head))
                    {
                        name = parameterInfo.GetCustomAttribute<RequestParamAttribute>().Name ?? parameterInfo.Name;
                        //replaceValueMethod = ReplaceRequestParamMethod;
                        replaceValueMethod = GetReplaceRequestParamMethod(typeBuilder);
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
                    replaceValueMethod = GetReplaceRequestQueryMethod(typeBuilder);
                }
                else
                {
                    name = parameterInfo.IsDefined(typeof(PathVariableAttribute)) ? parameterInfo.GetCustomAttribute<PathVariableAttribute>().Name : parameterInfo.Name;
                    //replaceValueMethod = ReplacePathVariableMethod;
                    replaceValueMethod = GetReplacePathVariableMethod(typeBuilder);
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = parameterInfo.Name;
                }

                iLGenerator.Emit(OpCodes.Ldstr, name);
                iLGenerator.Emit(OpCodes.Stloc, value);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldloc, uri);
                iLGenerator.Emit(OpCodes.Ldloc, value);
                iLGenerator.Emit(OpCodes.Ldarg_S, index);
                replaceValueMethod = replaceValueMethod.MakeGenericMethod(parameterInfo.ParameterType);
                iLGenerator.Emit(OpCodes.Call, replaceValueMethod);
                iLGenerator.Emit(OpCodes.Stloc, uri);

            }
            return emitRequestContents;
        }

    }
}
