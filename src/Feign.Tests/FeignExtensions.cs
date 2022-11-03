using Feign.Cache;
using Feign.Discovery;
using Feign.Fallback;
using Feign.Logging;
using Feign.Pipeline;
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
using System.Threading.Tasks;

namespace Feign.Tests
{

    public static class FeignExtensions
    {

        public static IFeignBuilder AddTestFeignClients(this IFeignBuilder feignBuilder)
        {
            feignBuilder.AddConverter(new TestServiceParamStringConverter());
            feignBuilder.AddServiceDiscovery<TestServiceDiscovery>();
            feignBuilder.Options.IncludeMethodMetadata = true;
            feignBuilder.AddFeignClients(Assembly.GetExecutingAssembly(), FeignClientLifetime.Transient);
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().UseSendingRequest(context =>
            {
                //context.Terminate();
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.Service<ITestControllerService>().UseSendingRequest(context =>
            {
                //e.CancellationTokenSource.Cancel();
                //e.Terminate();
                return TaskEx.CompletedTask;
            });

            feignBuilder.Options.FeignClientPipeline.Service<ITestControllerService>().UseCancelRequest(context =>
            {
                context.CancellationToken.Register(obj =>
                {
                    ICancelRequestPipelineContext<ITestControllerService> ee = obj as ICancelRequestPipelineContext<ITestControllerService>;
                    string s = ee.RequestMessage.ToString();
                }, context);
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().UseSendingRequest(context =>
            {
                var types = feignBuilder.Options.Types;
                if (context.RequestMessage.Content != null)
                {
                    MultipartFormDataContent multipartFormDataContent = context.RequestMessage.Content as MultipartFormDataContent;
                    if (multipartFormDataContent != null)
                    {
                        string boundary = multipartFormDataContent.Headers.ContentType.Parameters.FirstOrDefault(s => s.Name == "boundary").Value;
                        boundary = boundary.Replace("\"", "");
                        multipartFormDataContent.Headers.Remove("Content-Type");
                        multipartFormDataContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
                    }
                }
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.UseFallbackRequest(context =>
            {
                var parameters = context.GetParameters();
                object fallback = context.Fallback;
                IFallbackProxy fallbackProxy = context.FallbackProxy;
                if (fallbackProxy == null)
                {
                    //string s = "";
                }
                MethodInfo method = context.Method;
                context.Terminate();
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.UseInitializing(context =>
            {
                ((HttpClientHandler)context.HttpClient.Handler.InnerHandler).AutomaticDecompression =
                DecompressionMethods.None | DecompressionMethods.GZip | DecompressionMethods.Deflate;
            });
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().UseInitializing(context =>
            {
                context.FeignClient.Service.Name = "Initializing set";
                context.FeignClient.Service.ServiceId = context.FeignClient.ServiceId;
                context.FeignClient.Service.ServiceType = context.FeignClient.ServiceType;
            });
            feignBuilder.Options.FeignClientPipeline.Service("yun-platform-service-provider").UseInitializing(context =>
            {

            });
            feignBuilder.Options.FeignClientPipeline.UseDisposing(context =>
            {
                var ee = context;
            });
            //            feignBuilder.Options.FeignClientPipeline.Authorization(proxy =>
            //            {
            //#if NETSTANDARD
            //                return ("global", "asdasd");
            //#else
            //                return new AuthenticationHeaderValue("global", "asdasd");
            //#endif
            //            });
            feignBuilder.Options.FeignClientPipeline.UseBuildingRequest(FeignClientPipeline_BuildingRequest);
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().UseBuildingRequest(context =>
            {
                IFeignClient<ITestService> feignClient = context.FeignClient as IFeignClient<ITestService>;
                ITestService service = feignClient.Service;
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.Service("yun-platform-service-provider").UseBuildingRequest(context =>
            {
                var fallbackFeignClient = context.FeignClient.AsFallback<object>();
                fallbackFeignClient = context.FeignClient.AsFallback<object>();
                fallbackFeignClient = context.FeignClient.AsFallback<ITestService>();

                var fallback = fallbackFeignClient?.Fallback;

                fallback = context.FeignClient.GetFallback<object>();
                fallback = context.FeignClient.GetFallback<object>();
                //     fallback = e.FeignClient.GetFallback<ITestService>();

                if (!context.Headers.ContainsKey("Authorization"))
                {
                    context.Headers["Authorization"] = "service asdasd";
                }
                context.Headers["Accept-Encoding"] = "gzip, deflate, br";

                //add session
                context.Headers.Add("cookie", "csrftoken=EGxYkyZeT3DxEsvYsdR5ncmzpi9pmnQx; _bl_uid=nLjRstOyqOejLv2s0xtzqs74Xsmg; courseId=1; versionId=522; textbookId=2598; Hm_lvt_f0984c42ef98965e03c60661581cd219=1559783251,1559818390,1560213044,1560396804; uuid=6a30ff68-2b7c-4cde-a355-2e332b74e31d##1; Hm_lpvt_f0984c42ef98965e03c60661581cd219=1560413345; SESSION=5ee4854d-34b7-423a-9cca-76ddc8a0f111; sid=5ee4854d-34b7-423a-9cca-76ddc8a0f111");
                return TaskEx.CompletedTask;
            });
            //            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().Authorization(proxy =>
            //            {
            //#if NETSTANDARD
            //                return ("service", "asdasd");
            //#else
            //                return new AuthenticationHeaderValue("service", "asdasd");
            //#endif
            //            });
            feignBuilder.Options.FeignClientPipeline.UseSendingRequest(FeignClientPipeline_SendingRequest);
            feignBuilder.Options.FeignClientPipeline.Service("yun-platform-service-provider").UseReceivingResponse(context =>
            {
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.Service<ITestService>().ReceivingQueryResult();
            feignBuilder.Options.FeignClientPipeline.UseCancelRequest(context =>
            {
                context.CancellationToken.Register((state) =>
                {

                }, context.FeignClient);
                return TaskEx.CompletedTask;
            });
            feignBuilder.Options.FeignClientPipeline.UseErrorRequest(context =>
            {
                Exception exception = context.Exception;
                //e.ExceptionHandled = true;
                return TaskEx.CompletedTask;
            });
            return feignBuilder;
        }

        private static Task FeignClientPipeline_BuildingRequest(IBuildingRequestPipelineContext<object> context)
        {
            return TaskEx.CompletedTask;
        }

        private static Task FeignClientPipeline_SendingRequest(ISendingRequestPipelineContext<object> context)
        {
            //context.Terminate();
            return TaskEx.CompletedTask;
        }


        public static void ReceivingQueryResult<T>(this IFeignClientPipeline<T> globalFeignClient)
        {
            globalFeignClient.UseReceivingResponse(async context =>
            {
                if (!typeof(IQueryResult).IsAssignableFrom(context.ResultType))
                {
                    return;
                }
                if (context.ResultType == typeof(IQueryResult))
                {
                    context.Result = new QueryResult()
                    {
                        StatusCode = context.ResponseMessage.StatusCode
                    };
                    return;
                }

                var feignHttpRequestMessage = context.ResponseMessage.RequestMessage as Feign.Request.FeignHttpRequestMessage;

                var resultType = feignHttpRequestMessage.FeignClientRequest.Method.ResultType;


                if (context.ResultType.IsGenericType && context.ResultType.GetGenericTypeDefinition() == typeof(IQueryResult<>))
                {
                    QueryResult queryResult;
                    if (context.ResponseMessage.IsSuccessStatusCode)
                    {
                        var content = context.ResponseMessage.Content;
                        var buffer = await content.ReadAsByteArrayAsync();
                        var json1 = Encoding.GetEncoding("iso-8859-1").GetString(buffer);
                        string json = content.ReadAsStringAsync().Result;
                        object data = Newtonsoft.Json.JsonConvert.DeserializeObject(json, context.ResultType.GetGenericArguments()[0]);
                        if (data == null)
                        {
                            queryResult = InvokeQueryResultConstructor(context.ResultType.GetGenericArguments()[0]);
                        }
                        else
                        {
                            queryResult = InvokeQueryResultConstructor(data.GetType(), data);
                        }
                    }
                    else
                    {
                        queryResult = InvokeQueryResultConstructor(context.ResultType.GetGenericArguments()[0]);
                    }
                    queryResult.StatusCode = context.ResponseMessage.StatusCode;
                    context.Result = queryResult;
                }

                else if (context.ResultType.IsGenericType && context.ResultType.GetGenericTypeDefinition() == typeof(QueryResult<>))
                {
                    QueryResult queryResult;
                    if (context.ResponseMessage.IsSuccessStatusCode)
                    {
                        var content = context.ResponseMessage.Content;
                        var buffer = await content.ReadAsByteArrayAsync();
                        var json1 = Encoding.GetEncoding("iso-8859-1").GetString(buffer);
                        string json = content.ReadAsStringAsync().Result;
                        object data = Newtonsoft.Json.JsonConvert.DeserializeObject(json, context.ResultType.GetGenericArguments()[0]);
                        if (data == null)
                        {
                            queryResult = InvokeQueryResultConstructor(context.ResultType.GetGenericArguments()[0]);
                        }
                        else
                        {
                            queryResult = InvokeQueryResultConstructor(data.GetType(), data);
                        }
                    }
                    else
                    {
                        queryResult = InvokeQueryResultConstructor(context.ResultType.GetGenericArguments()[0]);
                    }
                    queryResult.StatusCode = context.ResponseMessage.StatusCode;
                    context.Result = queryResult;
                }
            });
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, QueryResult>> _newQueryResultMap = new System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, QueryResult>>();

        private static Func<QueryResult> _queryResultFunc;

        private static QueryResult InvokeQueryResultConstructor(Type type, object value)
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

        private static QueryResult InvokeQueryResultConstructor(Type type)
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
