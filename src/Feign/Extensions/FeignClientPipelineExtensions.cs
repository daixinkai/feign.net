using Feign.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FeignClientPipelineExtensions
    {
        /// <summary>
        /// Gets the specified service Pipeline
        /// </summary>
        /// <param name="globalFeignClientPipeline"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<object> Service(this IGlobalFeignClientPipeline globalFeignClientPipeline, string serviceId)
        {
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                throw new ArgumentException(nameof(serviceId));
            }
            return globalFeignClientPipeline.GetOrAddServicePipeline(serviceId);
        }


        /// <summary>
        /// Gets the specified service Pipeline
        /// </summary>
        /// <param name="globalFeignClientPipeline"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Service<TService>(this IGlobalFeignClientPipeline globalFeignClientPipeline)
        {
            return globalFeignClientPipeline.GetOrAddServicePipeline<TService>();
        }

        #region Authorization

        /// <summary>
        /// 添加授权
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="feignClientPipeline"></param>
        /// <param name="authenticationHeaderValue"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Authorization<TService>(this IFeignClientPipeline<TService> feignClientPipeline, AuthenticationHeaderValue authenticationHeaderValue)
        {
            if (authenticationHeaderValue == null)
            {
                throw new ArgumentNullException(nameof(authenticationHeaderValue));
            }
            return feignClientPipeline.UseBuildingRequest(context =>
            {
                if (!context.Headers.ContainsKey("Authorization"))
                {
                    context.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
                }
                return TaskEx.CompletedTask;
            });
        }
        /// <summary>
        /// 添加授权
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="feignClientPipeline"></param>
        /// <param name="authenticationHeaderValueAction"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Authorization<TService>(this IFeignClientPipeline<TService> feignClientPipeline, Func<IFeignClient<TService>, AuthenticationHeaderValue> authenticationHeaderValueAction)
        {
            if (authenticationHeaderValueAction == null)
            {
                throw new ArgumentNullException(nameof(authenticationHeaderValueAction));
            }
            return feignClientPipeline.UseBuildingRequest(context =>
            {
                if (!context.Headers.ContainsKey("Authorization"))
                {
                    var authenticationHeaderValue = authenticationHeaderValueAction.Invoke(context.FeignClient);
                    context.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
                }
                return TaskEx.CompletedTask;
            });
        }
        /// <summary>
        /// 添加授权
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="feignClientPipeline"></param>
        /// <param name="scheme"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Authorization<TService>(this IFeignClientPipeline<TService> feignClientPipeline, string scheme, string parameter)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            return feignClientPipeline.UseBuildingRequest(context =>
            {
                if (!context.Headers.ContainsKey("Authorization"))
                {
                    context.Headers["Authorization"] = scheme + " " + parameter;
                }
                return TaskEx.CompletedTask;
            });
        }

#if !NET45
        /// <summary>
        /// 添加授权
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="feignClientPipeline"></param>
        /// <param name="schemeAndParameterFactory"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Authorization<TService>(this IFeignClientPipeline<TService> feignClientPipeline, Func<IFeignClient<TService>, (string, string)> schemeAndParameterFactory)
        {
            if (schemeAndParameterFactory == null)
            {
                throw new ArgumentNullException(nameof(schemeAndParameterFactory));
            }
            return feignClientPipeline.UseBuildingRequest(context =>
            {
                if (!context.Headers.ContainsKey("Authorization"))
                {
                    var schemeAndParameter = schemeAndParameterFactory.Invoke(context.FeignClient);
                    context.Headers["Authorization"] = schemeAndParameter.Item1 + " " + schemeAndParameter.Item2;
                }
                return TaskEx.CompletedTask;
            });
        }
#endif


        #region Global
        /// <summary>
        /// 添加全局授权
        /// </summary>
        /// <param name="feignClientPipeline"></param>
        /// <param name="authenticationHeaderValue"></param>
        /// <returns></returns>
        public static IGlobalFeignClientPipeline Authorization(this IGlobalFeignClientPipeline feignClientPipeline, AuthenticationHeaderValue authenticationHeaderValue)
        {
            ((IFeignClientPipeline<object>)feignClientPipeline).Authorization(authenticationHeaderValue);
            return feignClientPipeline;
        }
        /// <summary>
        /// 添加全局授权
        /// </summary>
        /// <param name="feignClientPipeline"></param>
        /// <param name="authenticationHeaderValueAction"></param>
        /// <returns></returns>
        public static IGlobalFeignClientPipeline Authorization(this IGlobalFeignClientPipeline feignClientPipeline, Func<IFeignClient<object>, AuthenticationHeaderValue> authenticationHeaderValueAction)
        {
            ((IFeignClientPipeline<object>)feignClientPipeline).Authorization(authenticationHeaderValueAction);
            return feignClientPipeline;
        }
        /// <summary>
        /// 添加全局授权
        /// </summary>
        /// <param name="feignClientPipeline"></param>
        /// <param name="scheme"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static IGlobalFeignClientPipeline Authorization(this IGlobalFeignClientPipeline feignClientPipeline, string scheme, string parameter)
        {
            ((IFeignClientPipeline<object>)feignClientPipeline).Authorization(scheme, parameter);
            return feignClientPipeline;
        }

#if NETSTANDARD
        /// <summary>
        /// 添加全局授权
        /// </summary>
        /// <param name="feignClientPipeline"></param>
        /// <param name="schemeAndParameterFactory"></param>
        /// <returns></returns>
        public static IGlobalFeignClientPipeline Authorization(this IGlobalFeignClientPipeline feignClientPipeline, Func<IFeignClient<object>, (string, string)> schemeAndParameterFactory)
        {
            ((IFeignClientPipeline<object>)feignClientPipeline).Authorization(schemeAndParameterFactory);
            return feignClientPipeline;
        }
#endif

        #endregion


        //        public static T Authorization<T, TService>(this T feignClientPipeline, AuthenticationHeaderValue authenticationHeaderValue) where T : IFeignClientPipeline<TService>
        //        {
        //            if (authenticationHeaderValue == null)
        //            {
        //                throw new ArgumentNullException(nameof(authenticationHeaderValue));
        //            }
        //            feignClientPipeline.BuildingRequest += (sender, e) =>
        //            {
        //                if (!e.Headers.ContainsKey("Authorization"))
        //                {
        //                    e.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
        //                }
        //            };
        //            return feignClientPipeline;
        //        }
        //        public static T Authorization<T, TService>(this T feignClientPipeline, Func<IFeignClient<TService>, AuthenticationHeaderValue> authenticationHeaderValueAction) where T : IFeignClientPipeline<TService>
        //        {
        //            if (authenticationHeaderValueAction == null)
        //            {
        //                throw new ArgumentNullException(nameof(authenticationHeaderValueAction));
        //            }
        //            feignClientPipeline.BuildingRequest += (sender, e) =>
        //            {
        //                if (!e.Headers.ContainsKey("Authorization"))
        //                {
        //                    var authenticationHeaderValue = authenticationHeaderValueAction.Invoke(e.FeignClient);
        //                    e.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
        //                }
        //            };
        //            return feignClientPipeline;
        //        }
        //        public static T Authorization<T, TService>(this T feignClientPipeline, string scheme, string parameter) where T : IFeignClientPipeline<TService>
        //        {
        //            if (scheme == null)
        //            {
        //                throw new ArgumentNullException(nameof(scheme));
        //            }
        //            if (parameter == null)
        //            {
        //                throw new ArgumentNullException(nameof(parameter));
        //            }
        //            feignClientPipeline.BuildingRequest += (sender, e) =>
        //            {
        //                if (!e.Headers.ContainsKey("Authorization"))
        //                {
        //                    e.Headers["Authorization"] = scheme + " " + parameter;
        //                }
        //            };
        //            return feignClientPipeline;
        //        }

        //#if NETSTANDARD
        //        public static T Authorization<T, TService>(this T feignClientPipeline, Func<IFeignClient<TService>, (string, string)> schemeAndParameterFactory) where T : IFeignClientPipeline<TService>
        //        {
        //            if (schemeAndParameterFactory == null)
        //            {
        //                throw new ArgumentNullException(nameof(schemeAndParameterFactory));
        //            }
        //            feignClientPipeline.BuildingRequest += (sender, e) =>
        //            {
        //                if (!e.Headers.ContainsKey("Authorization"))
        //                {
        //                    var schemeAndParameter = schemeAndParameterFactory.Invoke(e.FeignClient);
        //                    e.Headers["Authorization"] = schemeAndParameter.Item1 + " " + schemeAndParameter.Item2;
        //                }
        //            };
        //            return feignClientPipeline;
        //        }
        //#endif




        #endregion

    }
}
