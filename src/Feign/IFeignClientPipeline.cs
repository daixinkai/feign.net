
/* 项目“Feign (netstandard2.1)”的未合并的更改
在此之前:
using System;
在此之后:
using Feign;
using Feign;
using Feign.Pipeline;
using System;
*/
using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClientPipeline<TService>
    {
        bool Enabled { get; set; }
        IFeignClientPipeline<TService> UseBuildingRequest(BuildingRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseCancelRequest(CancelRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseDisposing(DisposingDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseErrorRequest(ErrorRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseFallbackRequest(FallbackRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseInitializing(InitializingDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseReceivingResponse(ReceivingResponseDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseReceivedResponse(ReceivedResponseDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseSendingRequest(SendingRequestDelegate<TService> middleware);
    }
}
