using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class TaskExtensions
    {
        public static TResult GetResult<TResult>(this Task<TResult> task)
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                return task.Result;
            }

            return task
#if CONFIGUREAWAIT_FALSE
               .ConfigureAwait(false)
#endif
                .GetAwaiter().GetResult();
        }

        public static TResult GetResult<TResult>(this ConfiguredTaskAwaitable<TResult> configuredTaskAwaitable)
        {
            return configuredTaskAwaitable.GetAwaiter().GetResult();
        }


        public static void WaitEx(this Task task)
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                return;
            }
            task
#if CONFIGUREAWAIT_FALSE
             .ConfigureAwait(false)
#endif
              .GetAwaiter().GetResult();
        }

    }
}
