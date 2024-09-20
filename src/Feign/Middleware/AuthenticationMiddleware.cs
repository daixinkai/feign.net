using Feign.Internal;
using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public class AuthenticationMiddleware<T> : IBuildingRequestMiddleware<T>
    {

        enum ValueMode
        {
            AuthenticationHeaderValue,
            AuthenticationHeaderValueAction,
#if !NET45
            SchemeAndParameterFactory
#endif
        }

        public AuthenticationMiddleware(AuthenticationHeaderValue authenticationHeaderValue)
        {
            AuthenticationHeaderValue = authenticationHeaderValue;
            _mode = ValueMode.AuthenticationHeaderValue;
        }
        public AuthenticationMiddleware(Func<IFeignClient<T>, AuthenticationHeaderValue?> authenticationHeaderValueAction)
        {
            AuthenticationHeaderValueAction = authenticationHeaderValueAction;
            _mode = ValueMode.AuthenticationHeaderValueAction;
        }
#if !NET45
        public AuthenticationMiddleware(Func<IFeignClient<T>, (string, string)> schemeAndParameterFactory)
        {
            SchemeAndParameterFactory = schemeAndParameterFactory;
            _mode = ValueMode.SchemeAndParameterFactory;
        }
#endif

        private ValueMode _mode;

        public AuthenticationHeaderValue? AuthenticationHeaderValue { get; }

        public Func<IFeignClient<T>, AuthenticationHeaderValue?>? AuthenticationHeaderValueAction { get; }

#if !NET45
        public Func<IFeignClient<T>, (string, string)>? SchemeAndParameterFactory { get; }
#endif

        public ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context)
        {
            if (context.Headers.ContainsKey("Authorization"))
            {
                return TaskEx.CompletedValueTask;
            }

            switch (_mode)
            {
                case ValueMode.AuthenticationHeaderValue:
                    context.Headers["Authorization"] = AuthenticationHeaderValue!.Scheme + " " + AuthenticationHeaderValue!.Parameter;
                    break;
                case ValueMode.AuthenticationHeaderValueAction:
                    var authenticationHeaderValue = AuthenticationHeaderValueAction!.Invoke(context.FeignClient);
                    if (authenticationHeaderValue != null)
                    {
                        context.Headers["Authorization"] = authenticationHeaderValue.Scheme + " " + authenticationHeaderValue.Parameter;
                    }
                    break;
#if !NET45
                case ValueMode.SchemeAndParameterFactory:
                    var schemeAndParameter = SchemeAndParameterFactory!.Invoke(context.FeignClient);
                    context.Headers["Authorization"] = schemeAndParameter.Item1 + " " + schemeAndParameter.Item2;
                    break;
#endif
                default:
                    break;
            }

            return TaskEx.CompletedValueTask;
        }
    }
}
