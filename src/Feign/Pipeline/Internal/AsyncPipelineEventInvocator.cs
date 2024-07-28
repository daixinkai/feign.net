//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Feign.Pipeline
//{
//    internal readonly struct AsyncPipelineEventInvocator<T>
//    {
//        private readonly Func<T, ValueTask> _asyncHandler;

//        public AsyncPipelineEventInvocator(Func<T, ValueTask> asyncHandler)
//        {
//            _asyncHandler = asyncHandler;
//        }

//        public bool WrapsHandler(Func<T, ValueTask> handler)
//        {
//            // Do not use ReferenceEquals! It will not work with delegates.
//            return handler == _asyncHandler;
//        }

//        public ValueTask InvokeAsync(T eventArgs)
//        {
//            return _asyncHandler.Invoke(eventArgs);
//        }
//    }
//}
