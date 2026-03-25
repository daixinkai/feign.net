using Feign.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{

    internal abstract class FeignClientPipelineBase
    {
        public bool Enabled { get; set; } = true;

        public abstract bool HasMiddleware();

        public abstract void Add(FeignClientPipelineBase? pipeline);

    }

    internal abstract class FeignClientPipelineBase<TService> : FeignClientPipelineBase, IFeignClientPipeline<TService>
    {
        private readonly List<IBuildingRequestMiddleware<TService>> _buildingRequestMiddlewares = new();
        private readonly List<ICancelRequestMiddleware<TService>> _cancelRequestMiddlewares = new();
        private readonly List<IDisposingMiddleware<TService>> _disposingMiddlewares = new();
        private readonly List<IErrorRequestMiddleware<TService>> _errorRequestMiddlewares = new();
        private readonly List<IFallbackRequestMiddleware<TService>> _fallbackRequestMiddlewares = new();
        private readonly List<IInitializingMiddleware<TService>> _initializingMiddlewares = new();
        private readonly List<IReceivingResponseMiddleware<TService>> _receivingResponseMiddlewares = new();
        private readonly List<IReceivedResponseMiddleware<TService>> _receivedResponseMiddlewares = new();
        private readonly List<ISendingRequestMiddleware<TService>> _sendingRequestMiddlewares = new();

        public IFeignClientPipeline<TService> UseMiddleware(IFeignClientMiddleware<TService> middleware)
        {
            if (middleware is IBuildingRequestMiddleware<TService> buildingRequestMiddleware)
            {
                UseBuildingRequest(buildingRequestMiddleware);
            }
            if (middleware is ICancelRequestMiddleware<TService> cancelRequestMiddleware)
            {
                UseCancelRequest(cancelRequestMiddleware);
            }
            if (middleware is IDisposingMiddleware<TService> disposingMiddleware)
            {
                UseDisposing(disposingMiddleware);
            }
            if (middleware is IErrorRequestMiddleware<TService> errorRequestMiddleware)
            {
                UseErrorRequest(errorRequestMiddleware);
            }
            if (middleware is IFallbackRequestMiddleware<TService> fallbackRequestMiddleware)
            {
                UseFallbackRequest(fallbackRequestMiddleware);
            }
            if (middleware is IInitializingMiddleware<TService> initializingMiddleware)
            {
                UseInitializing(initializingMiddleware);
            }
            if (middleware is IReceivingResponseMiddleware<TService> receivingResponseMiddleware)
            {
                UseReceivingResponse(receivingResponseMiddleware);
            }
            if (middleware is IReceivedResponseMiddleware<TService> receivedResponseMiddleware)
            {
                UseReceivedResponse(receivedResponseMiddleware);
            }
            if (middleware is ISendingRequestMiddleware<TService> sendingRequestMiddleware)
            {
                UseSendingRequest(sendingRequestMiddleware);
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseBuildingRequest(BuildingRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseBuildingRequest(new DelegateBuildingRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseBuildingRequest(IBuildingRequestMiddleware<TService> middleware)
        {
            return AddMiddleware(_buildingRequestMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseCancelRequest(CancelRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseCancelRequest(new DelegateCancelRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseCancelRequest(ICancelRequestMiddleware<TService> middleware)
        {
            return AddMiddleware(_cancelRequestMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseDisposing(DisposingDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseDisposing(new DelegateDisposingMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseDisposing(IDisposingMiddleware<TService> middleware)
        {
            return AddMiddleware(_disposingMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseErrorRequest(ErrorRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseErrorRequest(new DelegateErrorRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseErrorRequest(IErrorRequestMiddleware<TService> middleware)
        {
            return AddMiddleware(_errorRequestMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseFallbackRequest(FallbackRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseFallbackRequest(new DelegateFallbackRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseFallbackRequest(IFallbackRequestMiddleware<TService> middleware)
        {
            return AddMiddleware(_fallbackRequestMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseInitializing(InitializingDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseInitializing(new DelegateInitializingMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseInitializing(IInitializingMiddleware<TService> middleware)
        {
            return AddMiddleware(_initializingMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseReceivingResponse(ReceivingResponseDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseReceivingResponse(new DelegateReceivingResponseMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseReceivingResponse(IReceivingResponseMiddleware<TService> middleware)
        {
            return AddMiddleware(_receivingResponseMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseReceivedResponse(ReceivedResponseDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseReceivedResponse(new DelegateReceivedResponseMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseReceivedResponse(IReceivedResponseMiddleware<TService> middleware)
        {
            return AddMiddleware(_receivedResponseMiddlewares, middleware);
        }

        public IFeignClientPipeline<TService> UseSendingRequest(SendingRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                UseSendingRequest(new DelegateSendingRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseSendingRequest(ISendingRequestMiddleware<TService> middleware)
        {
            return AddMiddleware(_sendingRequestMiddlewares, middleware);
        }

        protected internal virtual async Task BuildingRequestAsync(IBuildingRequestPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_buildingRequestMiddlewares))
            {
                return;
            }
            foreach (var middleware in _buildingRequestMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task SendingRequestAsync(ISendingRequestPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_sendingRequestMiddlewares))
            {
                return;
            }
            foreach (var middleware in _sendingRequestMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task CancelRequestAsync(ICancelRequestPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_cancelRequestMiddlewares))
            {
                return;
            }
            foreach (var middleware in _cancelRequestMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task ErrorRequestAsync(IErrorRequestPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_errorRequestMiddlewares))
            {
                return;
            }
            foreach (var middleware in _errorRequestMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task ReceivingResponseAsync(IReceivingResponsePipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_receivingResponseMiddlewares))
            {
                return;
            }
            foreach (var middleware in _receivingResponseMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual async Task ReceivedResponseAsync(IReceivedResponsePipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_receivedResponseMiddlewares))
            {
                return;
            }
            foreach (var middleware in _receivedResponseMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }
        protected internal virtual void Initializing(IInitializingPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_initializingMiddlewares))
            {
                return;
            }
            foreach (var middleware in _initializingMiddlewares)
            {
                middleware.Invoke(context);
            }
        }
        protected internal virtual void Disposing(IDisposingPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_disposingMiddlewares))
            {
                return;
            }
            foreach (var middleware in _disposingMiddlewares)
            {
                middleware.Invoke(context);
            }
        }
        protected internal virtual async Task FallbackRequestAsync(IFallbackRequestPipelineContext<TService> context)
        {
            if (!IsEnableMiddleware(_fallbackRequestMiddlewares))
            {
                return;
            }
            foreach (var middleware in _fallbackRequestMiddlewares)
            {
                await middleware.InvokeAsync(context)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
            }
        }


        public sealed override bool HasMiddleware()
        {
            return _buildingRequestMiddlewares.Count > 0
                || _cancelRequestMiddlewares.Count > 0
                || _disposingMiddlewares.Count > 0
                || _errorRequestMiddlewares.Count > 0
                || _fallbackRequestMiddlewares.Count > 0
                || _initializingMiddlewares.Count > 0
                || _receivingResponseMiddlewares.Count > 0
                || _receivedResponseMiddlewares.Count > 0
                || _sendingRequestMiddlewares.Count > 0;
        }

        public sealed override void Add(FeignClientPipelineBase? pipeline) 
            => Add(pipeline as FeignClientPipelineBase<TService>);

        public void Add(FeignClientPipelineBase<TService>? pipeline)
        {
            if (pipeline == null)
            {
                return;
            }
            _buildingRequestMiddlewares.AddRange(pipeline._buildingRequestMiddlewares);
            _cancelRequestMiddlewares.AddRange(pipeline._cancelRequestMiddlewares);
            _disposingMiddlewares.AddRange(pipeline._disposingMiddlewares);
            _errorRequestMiddlewares.AddRange(pipeline._errorRequestMiddlewares);
            _fallbackRequestMiddlewares.AddRange(pipeline._fallbackRequestMiddlewares);
            _initializingMiddlewares.AddRange(pipeline._initializingMiddlewares);
            _receivingResponseMiddlewares.AddRange(pipeline._receivingResponseMiddlewares);
            _receivedResponseMiddlewares.AddRange(pipeline._receivedResponseMiddlewares);
            _sendingRequestMiddlewares.AddRange(pipeline._sendingRequestMiddlewares);
        }

        private IFeignClientPipeline<TService> AddMiddleware<TMiddleware>(IList<TMiddleware> middlewares, TMiddleware? middleware) where TMiddleware : IFeignClientMiddleware
        {
            if (middleware != null)
            {
                middlewares.Add(middleware);
            }
            return this;
        }

        private bool IsEnableMiddleware<TMiddleware>(IList<TMiddleware> middlewares) where TMiddleware : IFeignClientMiddleware
        {
            if (!Enabled)
            {
                return false;
            }
            if (middlewares.Count == 0)
            {
                return false;
            }
            return true;
        }

    }
}
