using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    public delegate Task BuildingRequestDelegate<T>(IBuildingRequestPipelineContext<T> context);
    public delegate Task CancelRequestDelegate<T>(ICancelRequestPipelineContext<T> context);
    public delegate void DisposingDelegate<T>(IDisposingPipelineContext<T> context);
    public delegate Task ErrorRequestDelegate<T>(IErrorRequestPipelineContext<T> context);
    public delegate Task FallbackRequestDelegate<T>(IFallbackRequestPipelineContext<T> context);
    public delegate void InitializingDelegate<T>(IInitializingPipelineContext<T> context);
    public delegate Task ReceivingResponseDelegate<T>(IReceivingResponsePipelineContext<T> context);
    public delegate Task ReceivedResponseDelegate<T>(IReceivedResponsePipelineContext<T> context);
    public delegate Task SendingRequestDelegate<T>(ISendingRequestPipelineContext<T> context);
}
