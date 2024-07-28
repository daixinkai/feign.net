//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Feign.Pipeline
//{
//    internal class AsyncPipelineEvent<T>
//    {
//        private readonly List<AsyncPipelineEventInvocator<T>> _handlers = new();

//        private ICollection<AsyncPipelineEventInvocator<T>> _handlersForInvoke;

//        public AsyncPipelineEvent()
//        {
//            _handlersForInvoke = _handlers;
//        }

//        // Track the existence of handlers in a separate field so that checking it all the time will not
//        // require locking the actual list (_handlers).
//        public bool HasHandlers { get; private set; }

//        public void AddHandler(Func<T, ValueTask> handler)
//        {
//            if (handler == null)
//            {
//                throw new ArgumentNullException(nameof(handler));
//            }

//            lock (_handlers)
//            {
//                _handlers.Add(new AsyncPipelineEventInvocator<T>(handler));
//                HasHandlers = true;
//                _handlersForInvoke = new List<AsyncPipelineEventInvocator<T>>(_handlers);
//            }
//        }

//        public async ValueTask InvokeAsync(T eventArgs)
//        {
//            if (!HasHandlers)
//            {
//                return;
//            }

//            // Adding or removing handlers will produce a new list instance all the time.
//            // So locking here is not required since only the reference to an immutable list
//            // of handlers is used.
//            var handlers = _handlersForInvoke;
//            foreach (var handler in handlers)
//            {
//                await handler.InvokeAsync(eventArgs)
//#if USE_CONFIGUREAWAIT_FALSE
//                  .ConfigureAwait(false)
//#endif
//                  ;
//            }
//        }

//        public void RemoveHandler(Func<T, ValueTask> handler)
//        {
//            if (handler == null)
//            {
//                throw new ArgumentNullException(nameof(handler));
//            }
//            lock (_handlers)
//            {
//                _handlers.RemoveAll(h => h.WrapsHandler(handler));

//                HasHandlers = _handlers.Count > 0;
//                _handlersForInvoke = new List<AsyncPipelineEventInvocator<T>>(_handlers);
//            }
//        }

//        public async ValueTask TryInvokeAsync(T eventArgs)
//        {
//            if (eventArgs == null)
//            {
//                throw new ArgumentNullException(nameof(eventArgs));
//            }

//            try
//            {
//                await InvokeAsync(eventArgs)
//#if USE_CONFIGUREAWAIT_FALSE
//                  .ConfigureAwait(false)
//#endif
//                  ;
//            }
//            catch //(Exception exception)
//            {
//                //logger.Warning(exception, $"Error while invoking event with arguments of type {typeof(TEventArgs)}.");
//            }
//        }

//    }
//}
