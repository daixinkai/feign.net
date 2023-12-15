using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    partial class FeignClientHttpProxy<TService>
    {

        protected internal virtual async Task OnBuildingRequestAsync(IBuildingRequestPipelineContext<TService> context)
        {            
            if (_servicePipeline != null)
            {
                await _servicePipeline.BuildingRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.BuildingRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.BuildingRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnSendingRequestAsync(ISendingRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.SendingRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.SendingRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.SendingRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnCancelRequestAsync(ICancelRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.CancelRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.CancelRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.CancelRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnErrorRequestAsync(IErrorRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.ErrorRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.ErrorRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.ErrorRequestAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnReceivingResponseAsync(IReceivingResponsePipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.ReceivingResponseAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.ReceivingResponseAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.ReceivingResponseAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnReceivedResponseAsync(IReceivedResponsePipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.ReceivedResponseAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.ReceivedResponseAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.ReceivedResponseAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual void OnDisposing(IDisposingPipelineContext<TService> context)
        {
            _servicePipeline?.Disposing(context);
            _serviceIdPipeline?.Disposing(context);
            _globalPipeline?.Disposing(context);
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2214
        /// </summary>
        /// <param name="context"></param>
        private void OnInitializing(IInitializingPipelineContext<TService> context)
        {
            _servicePipeline?.Initializing(context);
            _serviceIdPipeline?.Initializing(context);
            _globalPipeline?.Initializing(context);
        }
    }
}
