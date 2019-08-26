using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    class FeignClientPipelineBase<TService> : IFeignClientPipeline<TService>
    {
        public virtual bool Enabled { get; set; } = true;
        public event EventHandler<IBuildingRequestEventArgs<TService>> BuildingRequest;
        public event EventHandler<ISendingRequestEventArgs<TService>> SendingRequest;
        public event EventHandler<ICancelRequestEventArgs<TService>> CancelRequest;
        public event EventHandler<IErrorRequestEventArgs<TService>> ErrorRequest;
        public event EventHandler<IReceivingResponseEventArgs<TService>> ReceivingResponse;
        public event EventHandler<IInitializingEventArgs<TService>> Initializing;
        public event EventHandler<IDisposingEventArgs<TService>> Disposing;
        public event EventHandler<IFallbackRequestEventArgs<TService>> FallbackRequest;

        protected internal virtual void OnBuildingRequest(object sender, IBuildingRequestEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            BuildingRequest?.Invoke(sender, e);
        }
        protected internal virtual void OnSendingRequest(object sender, ISendingRequestEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            SendingRequest?.Invoke(sender, e);
        }
        protected internal virtual void OnCancelRequest(object sender, ICancelRequestEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            CancelRequest?.Invoke(sender, e);
        }
        protected internal virtual void OnErrorRequest(object sender, IErrorRequestEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            ErrorRequest?.Invoke(sender, e);
        }
        protected internal virtual void OnReceivingResponse(object sender, IReceivingResponseEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            ReceivingResponse?.Invoke(sender, e);
        }
        protected internal virtual void OnInitializing(object sender, IInitializingEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            Initializing?.Invoke(sender, e);
        }
        protected internal virtual void OnDisposing(object sender, IDisposingEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            Disposing?.Invoke(sender, e);
        }
        protected internal virtual void OnFallbackRequest(object sender, IFallbackRequestEventArgs<TService> e)
        {
            if (!Enabled)
            {
                return;
            }
            FallbackRequest?.Invoke(sender, e);
        }
    }
}
