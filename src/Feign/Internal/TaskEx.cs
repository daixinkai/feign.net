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
#if NETSTANDARD
                return Task.CompletedTask;
#endif
#if NET45
                return Task.FromResult<object>(null);
#endif
            }
        }

    }
}
