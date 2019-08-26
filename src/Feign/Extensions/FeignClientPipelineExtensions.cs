using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
#if NET45
using FeignClientAuthorization = System.Tuple<string, string>;
#endif

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
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Service<TService>(this IGlobalFeignClientPipeline globalFeignClientPipeline)
        {
            return globalFeignClientPipeline.GetOrAddServicePipeline<TService>();
        }

        #region Authorization
        public static T Authorization<T, TService>(this T feignClientPipeline, AuthenticationHeaderValue authenticationHeaderValue) where T : IFeignClientPipeline<TService>
        {
            if (authenticationHeaderValue == null)
            {
                throw new ArgumentNullException(nameof(authenticationHeaderValue));
            }
            feignClientPipeline.BuildingRequest += (sender, e) =>
            {
                if (!e.Headers.ContainsKey("Authorization"))
                {
                    e.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
                }
            };
            return feignClientPipeline;
        }
        public static T Authorization<T, TService>(this T feignClientPipeline, Func<IFeignClient, AuthenticationHeaderValue> authenticationHeaderValueAction) where T : IFeignClientPipeline<TService>
        {
            if (authenticationHeaderValueAction == null)
            {
                throw new ArgumentNullException(nameof(authenticationHeaderValueAction));
            }
            feignClientPipeline.BuildingRequest += (sender, e) =>
            {
                if (!e.Headers.ContainsKey("Authorization"))
                {
                    var authenticationHeaderValue = authenticationHeaderValueAction.Invoke(e.FeignClient);
                    e.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
                }
            };
            return feignClientPipeline;
        }
        public static T Authorization<T, TService>(this T feignClientPipeline, string scheme, string parameter) where T : IFeignClientPipeline<TService>
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            feignClientPipeline.BuildingRequest += (sender, e) =>
            {
                if (!e.Headers.ContainsKey("Authorization"))
                {
                    e.Headers["Authorization"] = scheme + " " + parameter;
                }
            };
            return feignClientPipeline;
        }

#if NETSTANDARD
        public static T Authorization<T, TService>(this T feignClientPipeline, Func<IFeignClient, (string, string)> schemeAndParameterFactory) where T : IFeignClientPipeline<TService>
        {
            if (schemeAndParameterFactory == null)
            {
                throw new ArgumentNullException(nameof(schemeAndParameterFactory));
            }
            feignClientPipeline.BuildingRequest += (sender, e) =>
            {
                if (!e.Headers.ContainsKey("Authorization"))
                {
                    var schemeAndParameter = schemeAndParameterFactory.Invoke(e.FeignClient);
                    e.Headers["Authorization"] = schemeAndParameter.Item1 + " " + schemeAndParameter.Item2;
                }
            };
            return feignClientPipeline;
        }
#endif




        #endregion

    }
}
