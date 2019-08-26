using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Logging
{
    public class DefaultLogger : ILogger
    {
        public void LogDebug(string message, params object[] args)
        {
    
        }

        public void LogError(string message, params object[] args)
        {

        }

        public void LogError(Exception exception, string message, params object[] args)
        {

        }

        public void LogTrace(string message, params object[] args)
        {
  
        }

        public void LogWarning(string message, params object[] args)
        {

        }
    }
}
