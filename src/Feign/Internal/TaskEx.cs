using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    internal static class TaskEx
    {

        public static Task CompletedTask
        {
            get
            {
#if NET45
                return Task.FromResult<object?>(null);
#else
                return Task.CompletedTask;
#endif
            }
        }

        public static ValueTask CompletedValueTask
        {
            get
            {
#if USE_VALUE_TASK
                return default;
#else
                return CompletedTask;
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
