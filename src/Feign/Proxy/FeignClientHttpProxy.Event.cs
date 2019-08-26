using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    partial class FeignClientHttpProxy<TService>
    {

        protected internal virtual void OnBuildingRequest(BuildingRequestEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnBuildingRequest(this, e);
            _serviceIdFeignClientPipeline?.OnBuildingRequest(this, e);
            _globalFeignClientPipeline?.OnBuildingRequest(this, e);
        }
        protected internal virtual void OnSendingRequest(SendingRequestEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnSendingRequest(this, e);
            _serviceIdFeignClientPipeline?.OnSendingRequest(this, e);
            _globalFeignClientPipeline?.OnSendingRequest(this, e);
        }
        protected internal virtual void OnCancelRequest(CancelRequestEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnCancelRequest(this, e);
            _serviceIdFeignClientPipeline?.OnCancelRequest(this, e);
            _globalFeignClientPipeline?.OnCancelRequest(this, e);
        }
        protected internal virtual void OnErrorRequest(ErrorRequestEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnErrorRequest(this, e);
            _serviceIdFeignClientPipeline?.OnErrorRequest(this, e);
            _globalFeignClientPipeline?.OnErrorRequest(this, e);
        }
        protected internal virtual void OnReceivingResponse(ReceivingResponseEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnReceivingResponse(this, e);
            _serviceIdFeignClientPipeline?.OnReceivingResponse(this, e);
            _globalFeignClientPipeline?.OnReceivingResponse(this, e);
        }
        protected internal virtual void OnInitializing(InitializingEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnInitializing(this, e);
            _serviceIdFeignClientPipeline?.OnInitializing(this, e);
            _globalFeignClientPipeline?.OnInitializing(this, e);
        }
        protected internal virtual void OnDisposing(DisposingEventArgs<TService> e)
        {
            _serviceFeignClientPipeline?.OnDisposing(this, e);
            _serviceIdFeignClientPipeline?.OnDisposing(this, e);
            _globalFeignClientPipeline?.OnDisposing(this, e);
        }

    }
}
