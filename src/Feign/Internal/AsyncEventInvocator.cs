//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Feign.Internal
//{
//    internal readonly struct AsyncEventInvocator<TEventArgs>
//    {
//        private readonly Func<TEventArgs, Task> _asyncHandler;

//        public AsyncEventInvocator(Func<TEventArgs, Task> asyncHandler)
//        {
//            _asyncHandler = asyncHandler;
//        }

//        public Task InvokeAsync(TEventArgs eventArgs)
//        {
//            return _asyncHandler.Invoke(eventArgs);
//        }
//    }
//}
