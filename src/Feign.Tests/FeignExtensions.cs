﻿using Feign;
using Feign.Cache;
using Feign.Discovery;
using Feign.Fallback;
using Feign.Logging;
using Feign.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Feign.Tests
{

    public static class FeignExtensions
    {

        public static IFeignBuilder AddTestFeignClients(this IFeignBuilder feignBuilder)
        {
            Feign.FeignBuilderExtensions.AddConverter<TestServiceParam, string>(feignBuilder, new TestServiceParamStringConverter());
            feignBuilder.AddServiceDiscovery<TestServiceDiscovery>();
            feignBuilder.Options.IncludeMethodMetadata = true;
            Feign.FeignBuilderExtensions.AddFeignClients<IFeignBuilder>(feignBuilder, Assembly.GetExecutingAssembly(), FeignClientLifetime.Transient);
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().SendingRequest += (sender, e) =>
            {
                //e.Terminate();
            };
            feignBuilder.Options.FeignClientPipeline.Service<ITestControllerService>().SendingRequest += (sender, e) =>
            {
                //e.CancellationTokenSource.Cancel();
                //e.Terminate();
            };
            feignBuilder.Options.FeignClientPipeline.Service<ITestControllerService>().CancelRequest += (sender, e) =>
            {
                e.CancellationToken.Register(obj =>
                {
                    ICancelRequestEventArgs<ITestControllerService> ee = obj as ICancelRequestEventArgs<ITestControllerService>;
                    string s = ee.RequestMessage.ToString();
                }, e);
            };
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().SendingRequest += (sender, e) =>
            {
                var types = feignBuilder.Options.Types;
                if (e.RequestMessage.Content != null)
                {
                    MultipartFormDataContent multipartFormDataContent = e.RequestMessage.Content as MultipartFormDataContent;
                    if (multipartFormDataContent != null)
                    {
                        string boundary = multipartFormDataContent.Headers.ContentType.Parameters.FirstOrDefault(s => s.Name == "boundary").Value;
                        boundary = boundary.Replace("\"", "");
                        multipartFormDataContent.Headers.Remove("Content-Type");
                        multipartFormDataContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
                    }
                }
            };
            feignBuilder.Options.FeignClientPipeline.FallbackRequest += (sender, e) =>
            {
                var parameters = e.GetParameters();
                object fallback = e.Fallback;
                IFallbackProxy fallbackProxy = e.FallbackProxy;
                if (fallbackProxy == null)
                {
                    string s = "";
                }
                MethodInfo method = e.Method;
                e.Terminate();
            };
            feignBuilder.Options.FeignClientPipeline.Initializing += (sender, e) =>
            {
                ((HttpClientHandler)e.HttpClient.Handler.InnerHandler).AutomaticDecompression = DecompressionMethods.None | DecompressionMethods.GZip | DecompressionMethods.Deflate;
            };
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().Initializing += (sender, e) =>
            {
                e.FeignClient.Service.Name = "Initializing set";
                e.FeignClient.Service.ServiceId = e.FeignClient.ServiceId;
                e.FeignClient.Service.ServiceType = e.FeignClient.ServiceType;
            };
            feignBuilder.Options.FeignClientPipeline.Service("yun-platform-service-provider").Initializing += (sender, e) =>
            {

            };
            feignBuilder.Options.FeignClientPipeline.Disposing += (sender, e) =>
            {
                var ee = e;
            };
            //            feignBuilder.Options.FeignClientPipeline.Authorization(proxy =>
            //            {
            //#if NETSTANDARD
            //                return ("global", "asdasd");
            //#else
            //                return new AuthenticationHeaderValue("global", "asdasd");
            //#endif
            //            });
            feignBuilder.Options.FeignClientPipeline.BuildingRequest += FeignClientPipeline_BuildingRequest;
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().BuildingRequest += (sender, e) =>
            {
                IFeignClient<ITestService> feignClient = e.FeignClient as IFeignClient<ITestService>;
                ITestService service = feignClient.Service;
            };
            feignBuilder.Options.FeignClientPipeline.Service("yun-platform-service-provider").BuildingRequest += (sender, e) =>
            {
                var fallbackFeignClient = e.FeignClient.AsFallback<object>();
                fallbackFeignClient = e.FeignClient.AsFallback<object>();
                fallbackFeignClient = e.FeignClient.AsFallback<ITestService>();

                var fallback = fallbackFeignClient?.Fallback;

                fallback = e.FeignClient.GetFallback<object>();
                fallback = e.FeignClient.GetFallback<object>();
                //     fallback = e.FeignClient.GetFallback<ITestService>();

                if (!e.Headers.ContainsKey("Authorization"))
                {
                    e.Headers["Authorization"] = "service asdasd";
                }
                e.Headers["Accept-Encoding"] = "gzip, deflate, br";

                //add session
                e.Headers.Add("cookie", "csrftoken=EGxYkyZeT3DxEsvYsdR5ncmzpi9pmnQx; _bl_uid=nLjRstOyqOejLv2s0xtzqs74Xsmg; courseId=1; versionId=522; textbookId=2598; Hm_lvt_f0984c42ef98965e03c60661581cd219=1559783251,1559818390,1560213044,1560396804; uuid=6a30ff68-2b7c-4cde-a355-2e332b74e31d##1; Hm_lpvt_f0984c42ef98965e03c60661581cd219=1560413345; SESSION=5ee4854d-34b7-423a-9cca-76ddc8a0f111; sid=5ee4854d-34b7-423a-9cca-76ddc8a0f111");

            };
            //            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().Authorization(proxy =>
            //            {
            //#if NETSTANDARD
            //                return ("service", "asdasd");
            //#else
            //                return new AuthenticationHeaderValue("service", "asdasd");
            //#endif
            //            });
            feignBuilder.Options.FeignClientPipeline.SendingRequest += FeignClientPipeline_SendingRequest;
            feignBuilder.Options.FeignClientPipeline.Service("yun-platform-service-provider").ReceivingResponse += (sender, e) =>
            {

            };
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().ReceivingQueryResult();
            feignBuilder.Options.FeignClientPipeline.CancelRequest += (sender, e) =>
            {
                e.CancellationToken.Register((state) =>
                {

                }, sender);
            };
            feignBuilder.Options.FeignClientPipeline.ErrorRequest += (sender, e) =>
            {
                Exception exception = e.Exception;
                //e.ExceptionHandled = true;
            };
            return feignBuilder;
        }

        private static void FeignClientPipeline_BuildingRequest(object sender, IBuildingRequestEventArgs<object> e)
        {
        }

        private static void FeignClientPipeline_SendingRequest(object sender, ISendingRequestEventArgs<object> e)
        {
            //e.Terminate();
        }


        public static void ReceivingQueryResult<T>(this IFeignClientPipeline<T> globalFeignClient)
        {
            globalFeignClient.ReceivingResponse += (sender, e) =>
            {
                if (!typeof(IQueryResult).IsAssignableFrom(e.ResultType))
                {
                    return;
                }
                if (e.ResultType == typeof(IQueryResult))
                {
                    e.Result = new QueryResult()
                    {
                        StatusCode = e.ResponseMessage.StatusCode
                    };
                    return;
                }

                Feign.Request.FeignHttpRequestMessage feignHttpRequestMessage = e.ResponseMessage.RequestMessage as Feign.Request.FeignHttpRequestMessage;

                var resultType = feignHttpRequestMessage.FeignClientRequest.Method.ResultType;


                if (e.ResultType.IsGenericType && e.ResultType.GetGenericTypeDefinition() == typeof(IQueryResult<>))
                {
                    QueryResult queryResult;
                    if (e.ResponseMessage.IsSuccessStatusCode)
                    {
                        var content = e.ResponseMessage.Content;
                        var buffer = content.ReadAsByteArrayAsync().Result;
                        var json1 = Encoding.GetEncoding("iso-8859-1").GetString(buffer);
                        string json = content.ReadAsStringAsync().Result;
                        object data = Newtonsoft.Json.JsonConvert.DeserializeObject(json, e.ResultType.GetGenericArguments()[0]);
                        if (data == null)
                        {
                            queryResult = InvokeQueryResultConstructor(e.ResultType.GetGenericArguments()[0]);
                        }
                        else
                        {
                            queryResult = InvokeQueryResultConstructor(data.GetType(), data);
                        }
                    }
                    else
                    {
                        queryResult = InvokeQueryResultConstructor(e.ResultType.GetGenericArguments()[0]);
                    }
                    queryResult.StatusCode = e.ResponseMessage.StatusCode;
                    e.Result = queryResult;
                }

                else if (e.ResultType.IsGenericType && e.ResultType.GetGenericTypeDefinition() == typeof(QueryResult<>))
                {
                    QueryResult queryResult;
                    if (e.ResponseMessage.IsSuccessStatusCode)
                    {
                        var content = e.ResponseMessage.Content;
                        var buffer = content.ReadAsByteArrayAsync().Result;
                        var json1 = Encoding.GetEncoding("iso-8859-1").GetString(buffer);
                        string json = content.ReadAsStringAsync().Result;
                        object data = Newtonsoft.Json.JsonConvert.DeserializeObject(json, e.ResultType.GetGenericArguments()[0]);
                        if (data == null)
                        {
                            queryResult = InvokeQueryResultConstructor(e.ResultType.GetGenericArguments()[0]);
                        }
                        else
                        {
                            queryResult = InvokeQueryResultConstructor(data.GetType(), data);
                        }
                    }
                    else
                    {
                        queryResult = InvokeQueryResultConstructor(e.ResultType.GetGenericArguments()[0]);
                    }
                    queryResult.StatusCode = e.ResponseMessage.StatusCode;
                    e.Result = queryResult;
                }

            };
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, QueryResult>> _newQueryResultMap = new System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, QueryResult>>();

        static Func<QueryResult> _queryResultFunc;

        static QueryResult InvokeQueryResultConstructor(Type type, object value)
        {
            Func<object, QueryResult> func = _newQueryResultMap.GetOrAdd(type, key =>
            {
                Type queryResultType = typeof(QueryResult<>).MakeGenericType(key);
                ConstructorInfo constructor = queryResultType.GetConstructor(new Type[] { key });
                ParameterExpression parameter = Expression.Parameter(typeof(object));
                NewExpression constructorExpression = Expression.New(constructor, Expression.Convert(parameter, key));
                return Expression.Lambda<Func<object, QueryResult>>(constructorExpression, parameter).Compile();
            });
            return func.Invoke(value);
        }

        static QueryResult InvokeQueryResultConstructor(Type type)
        {
            if (_queryResultFunc == null)
            {
                Type queryResultType = typeof(QueryResult<>).MakeGenericType(type);
                ConstructorInfo constructor = queryResultType.GetConstructor(Type.EmptyTypes);
                NewExpression constructorExpression = Expression.New(constructor);
                _queryResultFunc = Expression.Lambda<Func<QueryResult>>(constructorExpression).Compile();
            }
            return _queryResultFunc.Invoke();
        }

    }
}
