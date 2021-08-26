#if USE_SYSTEM_TEXT_JSON
using System.Text.Json;
using JsonSerializerSettings = System.Text.Json.JsonSerializerOptions;
#else
using Newtonsoft.Json;
#endif
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
        public static void ConfigureJsonSettings(this IFeignOptions feignOptions, Action<JsonSerializerSettings> configure)
        {

#if USE_SYSTEM_TEXT_JSON

            var setting = feignOptions.JsonProvider as SystemTextJsonProvider;
            if (setting != null)
            {
                configure?.Invoke(setting._jsonSerializerOptions);
            }
#else
            var setting = feignOptions.JsonProvider as NewtonsoftJsonProvider;
            if (setting != null)
            {
                configure?.Invoke(setting._jsonSerializerSettings);
            }
#endif


        }
    }
}
