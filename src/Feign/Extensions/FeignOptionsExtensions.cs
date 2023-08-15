using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feign.Formatting;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FeignOptionsExtensions
    {
        /// <summary>
        /// Configure JsonSettings
        /// </summary>
        /// <param name="feignOptions"></param>
        /// <param name="configure"></param>
        public static void ConfigureJsonSettings(this IFeignOptions feignOptions, Action<JsonSerializerOptions> configure)
        {
            var setting = feignOptions.JsonProvider as JsonProviderType;
            setting?.Configure(configure);
        }
    }
}
