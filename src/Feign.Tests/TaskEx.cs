#if !NETSTANDARD2_1 && !NETCOREAPP3_1_OR_GREATER
global using ValueTask = System.Threading.Tasks.Task;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    static class TaskEx
    {

        public static Task CompletedTask
        {
            get
            {
#if NET45
                return Task.FromResult<object>(null);
#else
                return Task.CompletedTask;
#endif
            }
        }

        public static ValueTask CompletedValueTask
        {
            get
            {
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
                return default;
#else
                return ValueTask.FromResult<object>(null);
#endif
            }
        }

        public static Task FromException(Exception exception)
        {
#if NET45
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            tcs.SetException(exception);
            return tcs.Task;
#else
            return Task.FromException(exception);
#endif
        }

    }
}
