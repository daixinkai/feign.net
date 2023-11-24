using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    internal class TerminatedRequestException : Exception, ISkipLogException
    {
        public TerminatedRequestException()
        {

        }

        public TerminatedRequestException(string message) : base(message)
        {
        }

        public TerminatedRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
