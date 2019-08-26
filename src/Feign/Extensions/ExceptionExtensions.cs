using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class ExceptionExtensions
    {
        public static bool IsSkipLog(this Exception exception)
        {
            return exception is ISkipLogException;
        }
    }
}
