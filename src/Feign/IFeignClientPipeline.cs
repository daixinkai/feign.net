using Feign.Middleware;
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
        /// <summary>
        /// Use middleware
        /// </summary>
        /// <param name="middleware">
        /// <para><see cref="IBuildingRequestMiddleware{TService}"/></para>
        /// <para><see cref="IBuildingRequestMiddleware{TService}"/></para>
        /// <para><see cref="ICancelRequestMiddleware{TService}"/></para>
        /// <para><see cref="IDisposingMiddleware{TService}"/></para>
        /// <para><see cref="IErrorRequestMiddleware{TService}"/></para>
        /// <para><see cref="IFallbackRequestMiddleware{TService}"/></para>
        /// <para><see cref="IInitializingMiddleware{TService}"/></para>
        /// <para><see cref="IReceivedResponseMiddleware{TService}"/></para>
        /// <para><see cref="IReceivingResponseMiddleware{TService}"/></para>
        /// <para><see cref="ISendingRequestMiddleware{TService}"/></para>
        /// </param>
        /// <returns></returns>
        IFeignClientPipeline<TService> UseMiddleware(IFeignClientMiddleware<TService> middleware);
        //IFeignClientPipeline<TService> UseBuildingRequest(IBuildingRequestMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseBuildingRequest(BuildingRequestDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseCancelRequest(ICancelRequestMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseCancelRequest(CancelRequestDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseDisposing(IDisposingMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseDisposing(DisposingDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseErrorRequest(IErrorRequestMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseErrorRequest(ErrorRequestDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseFallbackRequest(IFallbackRequestMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseFallbackRequest(FallbackRequestDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseInitializing(IInitializingMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseInitializing(InitializingDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseReceivingResponse(IReceivingResponseMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseReceivingResponse(ReceivingResponseDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseReceivedResponse(IReceivedResponseMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseReceivedResponse(ReceivedResponseDelegate<TService> middleware);
        //IFeignClientPipeline<TService> UseSendingRequest(ISendingRequestMiddleware<TService> middleware);
        IFeignClientPipeline<TService> UseSendingRequest(SendingRequestDelegate<TService> middleware);
    }
}
