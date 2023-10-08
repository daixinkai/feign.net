using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    public delegate ValueTask BuildingRequestDelegate<T>(IBuildingRequestPipelineContext<T> context);
    public delegate ValueTask CancelRequestDelegate<T>(ICancelRequestPipelineContext<T> context);
    public delegate void DisposingDelegate<T>(IDisposingPipelineContext<T> context);
    public delegate ValueTask ErrorRequestDelegate<T>(IErrorRequestPipelineContext<T> context);
    public delegate ValueTask FallbackRequestDelegate<T>(IFallbackRequestPipelineContext<T> context);
    public delegate void InitializingDelegate<T>(IInitializingPipelineContext<T> context);
    public delegate ValueTask ReceivingResponseDelegate<T>(IReceivingResponsePipelineContext<T> context);
    public delegate ValueTask ReceivedResponseDelegate<T>(IReceivedResponsePipelineContext<T> context);
    public delegate ValueTask SendingRequestDelegate<T>(ISendingRequestPipelineContext<T> context);
}
