using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IFeignClientPipeline<TService>
    {
        bool Enabled { get; set; }
        event EventHandler<IBuildingRequestEventArgs<TService>> BuildingRequest;
        event EventHandler<ISendingRequestEventArgs<TService>> SendingRequest;
        event EventHandler<ICancelRequestEventArgs<TService>> CancelRequest;
        event EventHandler<IErrorRequestEventArgs<TService>> ErrorRequest;
        event EventHandler<IReceivingResponseEventArgs<TService>> ReceivingResponse;
        event EventHandler<IInitializingEventArgs<TService>> Initializing;
        event EventHandler<IDisposingEventArgs<TService>> Disposing;
        event EventHandler<IFallbackRequestEventArgs<TService>> FallbackRequest;
    }
}
