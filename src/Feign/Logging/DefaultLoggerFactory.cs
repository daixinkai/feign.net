using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Logging
{
    public class DefaultLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(Type type)
        {
            return new DefaultLogger();
        }
    }
}
