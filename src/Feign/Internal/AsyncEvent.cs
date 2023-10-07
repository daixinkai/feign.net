//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Feign.Internal
//{
//    internal class AsyncEvent<TEventArgs>
//    {
//        readonly List<AsyncEventInvocator<TEventArgs>> _handlers = new List<AsyncEventInvocator<TEventArgs>>();

//        ICollection<AsyncEventInvocator<TEventArgs>> _handlersForInvoke;

//        public AsyncEvent()
//        {
//            _handlersForInvoke = _handlers;
//        }

//        // Track the existence of handlers in a separate field so that checking it all the time will not
//        // require locking the actual list (_handlers).
//        public bool HasHandlers { get; private set; }

//        public void AddHandler(Func<TEventArgs, Task> handler)
//        {
//            if (handler == null)
//            {
//                throw new ArgumentNullException(nameof(handler));
//            }

//            lock (_handlers)
//            {
//                _handlers.Add(new AsyncEventInvocator<TEventArgs>(handler));
//                HasHandlers = true;
//                _handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(_handlers);
//            }
//        }

//        public async Task InvokeAsync(TEventArgs eventArgs)
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
//                await handler.InvokeAsync(eventArgs).ConfigureAwait(false);
//            }
//        }


//        public async Task TryInvokeAsync(TEventArgs eventArgs)
//        {
//            if (eventArgs == null)
//            {
//                throw new ArgumentNullException(nameof(eventArgs));
//            }

//            try
//            {
//                await InvokeAsync(eventArgs).ConfigureAwait(false);
//            }
//            catch //(Exception exception)
//            {
//                //logger.Warning(exception, $"Error while invoking event with arguments of type {typeof(TEventArgs)}.");
//            }
//        }

//    }
//}
