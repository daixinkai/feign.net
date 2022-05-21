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
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.BuildingRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.BuildingRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.BuildingRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnSendingRequestAsync(ISendingRequestPipelineContext<TService> context)
        {
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.SendingRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.SendingRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.SendingRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnCancelRequestAsync(ICancelRequestPipelineContext<TService> context)
        {
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.CancelRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.CancelRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.CancelRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnErrorRequestAsync(IErrorRequestPipelineContext<TService> context)
        {
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.ErrorRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.ErrorRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.ErrorRequestAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnReceivingResponseAsync(IReceivingResponsePipelineContext<TService> context)
        {
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.ReceivingResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.ReceivingResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.ReceivingResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task OnReceivedResponseAsync(IReceivedResponsePipelineContext<TService> context)
        {
            if (_serviceFeignClientPipeline != null)
            {
                await _serviceFeignClientPipeline.ReceivedResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_serviceIdFeignClientPipeline != null)
            {
                await _serviceIdFeignClientPipeline.ReceivedResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
            if (_globalFeignClientPipeline != null)
            {
                await _globalFeignClientPipeline.ReceivedResponseAsync(context)
#if CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual void OnInitializing(IInitializingPipelineContext<TService> context)
        {
            _serviceFeignClientPipeline?.Initializing(context);
            _serviceIdFeignClientPipeline?.Initializing(context);
            _globalFeignClientPipeline?.Initializing(context);
        }
        protected internal virtual void OnDisposing(IDisposingPipelineContext<TService> context)
        {
            _serviceFeignClientPipeline?.Disposing(context);
            _serviceIdFeignClientPipeline?.Disposing(context);
            _globalFeignClientPipeline?.Disposing(context);
        }

    }
}
