using System;
using System.Collections.Generic;
using System.Linq;
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
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
