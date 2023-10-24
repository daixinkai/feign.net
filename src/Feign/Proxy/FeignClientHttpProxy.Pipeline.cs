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

        protected internal virtual async ValueTask OnBuildingRequestAsync(IBuildingRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.BuildingRequestAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.BuildingRequestAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.BuildingRequestAsync(context).ConfigureAwait(false);
            }
        }
        protected internal virtual async ValueTask OnSendingRequestAsync(ISendingRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.SendingRequestAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.SendingRequestAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.SendingRequestAsync(context).ConfigureAwait(false);
            }
        }
        protected internal virtual async ValueTask OnCancelRequestAsync(ICancelRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.CancelRequestAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.CancelRequestAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.CancelRequestAsync(context).ConfigureAwait(false);
            }
        }
        protected internal virtual async ValueTask OnErrorRequestAsync(IErrorRequestPipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.ErrorRequestAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.ErrorRequestAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.ErrorRequestAsync(context).ConfigureAwait(false);
            }
        }
        protected internal virtual async ValueTask OnReceivingResponseAsync(IReceivingResponsePipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.ReceivingResponseAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.ReceivingResponseAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.ReceivingResponseAsync(context).ConfigureAwait(false);
            }
        }
        protected internal virtual async ValueTask OnReceivedResponseAsync(IReceivedResponsePipelineContext<TService> context)
        {
            if (_servicePipeline != null)
            {
                await _servicePipeline.ReceivedResponseAsync(context).ConfigureAwait(false);
            }
            if (_serviceIdPipeline != null)
            {
                await _serviceIdPipeline.ReceivedResponseAsync(context).ConfigureAwait(false);
            }
            if (_globalPipeline != null)
            {
                await _globalPipeline.ReceivedResponseAsync(context).ConfigureAwait(false);
            }
        }
        protected internal virtual void OnInitializing(IInitializingPipelineContext<TService> context)
        {
            _servicePipeline?.Initializing(context);
            _serviceIdPipeline?.Initializing(context);
            _globalPipeline?.Initializing(context);
        }
        protected internal virtual void OnDisposing(IDisposingPipelineContext<TService> context)
        {
            _servicePipeline?.Disposing(context);
            _serviceIdPipeline?.Disposing(context);
            _globalPipeline?.Disposing(context);
        }

    }
}
