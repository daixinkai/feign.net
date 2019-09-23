using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    class ServiceResolveFailException : Exception, ISkipLogException
    {
        public ServiceResolveFailException()
        {

        }

        public ServiceResolveFailException(string message) : base(message)
        {
        }

        public ServiceResolveFailException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServiceResolveFailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
