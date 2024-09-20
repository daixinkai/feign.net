using Feign.Internal;
using Feign.Middleware;
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
        #region Authorization

        /// <summary>
        /// Add authorization
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
            return feignClientPipeline.UseMiddleware(new AuthenticationMiddleware<TService>(authenticationHeaderValue));
        }
        /// <summary>
        /// Add authorization
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="feignClientPipeline"></param>
        /// <param name="authenticationHeaderValueAction"></param>
        /// <returns></returns>
        public static IFeignClientPipeline<TService> Authorization<TService>(this IFeignClientPipeline<TService> feignClientPipeline, Func<IFeignClient<TService>, AuthenticationHeaderValue?> authenticationHeaderValueAction)
        {
            if (authenticationHeaderValueAction == null)
            {
                throw new ArgumentNullException(nameof(authenticationHeaderValueAction));
            }
            return feignClientPipeline.UseMiddleware(new AuthenticationMiddleware<TService>(authenticationHeaderValueAction));
        }
        /// <summary>
        /// Add authorization
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
            return feignClientPipeline.Authorization(new AuthenticationHeaderValue(scheme, parameter));
        }

#if !NET45
        /// <summary>
        /// Add authorization
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
            return feignClientPipeline.UseMiddleware(new AuthenticationMiddleware<TService>(schemeAndParameterFactory));
        }
#endif


        #region Global
        /// <summary>
        /// Add global authorization
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
        /// Add global authorization
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
        /// Add global authorization
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

#if !NET45
        /// <summary>
        /// Add global authorization
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

        #endregion

    }
}
