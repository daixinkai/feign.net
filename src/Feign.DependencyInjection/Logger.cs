using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Logging
{
    class Logger : Feign.Logging.ILogger
    {
        public Logger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        Microsoft.Extensions.Logging.ILogger _logger;

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }
    }
}
