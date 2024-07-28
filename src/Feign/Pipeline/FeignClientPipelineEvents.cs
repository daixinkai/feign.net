//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Feign.Pipeline
//{
//    public sealed class FeignClientPipelineEvents<TService>
//    {
//        private readonly AsyncPipelineEvent<IBuildingRequestPipelineContext<TService>> _buildingRequestEvent = new();
//        private readonly AsyncPipelineEvent<ICancelRequestPipelineContext<TService>> _cancelRequestEvent = new();
//        private readonly AsyncPipelineEvent<IErrorRequestPipelineContext<TService>> _errorRequestEvent = new();
//        private readonly AsyncPipelineEvent<IFallbackRequestPipelineContext<TService>> _fallbackRequest = new();
//        private readonly AsyncPipelineEvent<IReceivingResponsePipelineContext<TService>> _receivingResponseEvent = new();
//        private readonly AsyncPipelineEvent<IReceivedResponsePipelineContext<TService>> _receivedResponseEvent = new();
//        private readonly AsyncPipelineEvent<ISendingRequestPipelineContext<TService>> _sendingRequestEvent = new();

//        public event Action<IDisposingPipelineContext<TService>> Disposing = null!;
//        public event Action<IInitializingPipelineContext<TService>> Initializing = null!;


//        public event Func<IBuildingRequestPipelineContext<TService>, ValueTask> BuildingRequest
//        {
//            add => _buildingRequestEvent.AddHandler(value);
//            remove => _buildingRequestEvent.RemoveHandler(value);
//        }

//        public event Func<ICancelRequestPipelineContext<TService>, ValueTask> CancelRequest
//        {
//            add => _cancelRequestEvent.AddHandler(value);
//            remove => _cancelRequestEvent.RemoveHandler(value);
//        }

//        public event Func<IErrorRequestPipelineContext<TService>, ValueTask> ErrorRequest
//        {
//            add => _errorRequestEvent.AddHandler(value);
//            remove => _errorRequestEvent.RemoveHandler(value);
//        }

//        public event Func<IFallbackRequestPipelineContext<TService>, ValueTask> FallbackRequest
//        {
//            add => _fallbackRequest.AddHandler(value);
//            remove => _fallbackRequest.RemoveHandler(value);
//        }

//        public event Func<IReceivingResponsePipelineContext<TService>, ValueTask> ReceivingResponse
//        {
//            add => _receivingResponseEvent.AddHandler(value);
//            remove => _receivingResponseEvent.RemoveHandler(value);
//        }

//        public event Func<IReceivedResponsePipelineContext<TService>, ValueTask> ReceivedResponse
//        {
//            add => _receivedResponseEvent.AddHandler(value);
//            remove => _receivedResponseEvent.RemoveHandler(value);
//        }

//        public event Func<ISendingRequestPipelineContext<TService>, ValueTask> SendingRequest
//        {
//            add => _sendingRequestEvent.AddHandler(value);
//            remove => _sendingRequestEvent.RemoveHandler(value);
//        }
//    }
//}
