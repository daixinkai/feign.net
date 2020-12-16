using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
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
