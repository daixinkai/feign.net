﻿using Feign.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal class FeignClientPipelineBase<TService> : IFeignClientPipeline<TService>
    {
        public virtual bool Enabled { get; set; } = true;

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
                _buildingRequestMiddlewares.Add(new DelegateBuildingRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseBuildingRequest(IBuildingRequestMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _buildingRequestMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseCancelRequest(CancelRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _cancelRequestMiddlewares.Add(new DelegateCancelRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseCancelRequest(ICancelRequestMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _cancelRequestMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseDisposing(DisposingDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _disposingMiddlewares.Add(new DelegateDisposingMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseDisposing(IDisposingMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _disposingMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseErrorRequest(ErrorRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _errorRequestMiddlewares.Add(new DelegateErrorRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseErrorRequest(IErrorRequestMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _errorRequestMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseFallbackRequest(FallbackRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _fallbackRequestMiddlewares.Add(new DelegateFallbackRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseFallbackRequest(IFallbackRequestMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _fallbackRequestMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseInitializing(InitializingDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _initializingMiddlewares.Add(new DelegateInitializingMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseInitializing(IInitializingMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _initializingMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseReceivingResponse(ReceivingResponseDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _receivingResponseMiddlewares.Add(new DelegateReceivingResponseMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseReceivingResponse(IReceivingResponseMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _receivingResponseMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseReceivedResponse(ReceivedResponseDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _receivedResponseMiddlewares.Add(new DelegateReceivedResponseMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseReceivedResponse(IReceivedResponseMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _receivedResponseMiddlewares.Add(middleware);
            }
            return this;
        }

        public IFeignClientPipeline<TService> UseSendingRequest(SendingRequestDelegate<TService> middleware)
        {
            if (middleware != null)
            {
                _sendingRequestMiddlewares.Add(new DelegateSendingRequestMiddleware<TService>(middleware));
            }
            return this;
        }
        public IFeignClientPipeline<TService> UseSendingRequest(ISendingRequestMiddleware<TService> middleware)
        {
            if (middleware != null)
            {
                _sendingRequestMiddlewares.Add(middleware);
            }
            return this;
        }

        protected internal virtual async Task BuildingRequestAsync(IBuildingRequestPipelineContext<TService> context)
        {
            if (!Enabled)
            {
                return;
            }
            if (_buildingRequestMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_sendingRequestMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_cancelRequestMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_errorRequestMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_receivingResponseMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_receivedResponseMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_initializingMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_disposingMiddlewares.Count == 0)
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
            if (!Enabled)
            {
                return;
            }
            if (_fallbackRequestMiddlewares.Count == 0)
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


        public bool HasMiddleware()
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


    }
}
