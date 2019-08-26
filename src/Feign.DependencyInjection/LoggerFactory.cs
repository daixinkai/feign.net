using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Feign.Logging
{
    class LoggerFactory : Feign.Logging.ILoggerFactory
    {
        public LoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

        public Feign.Logging.ILogger CreateLogger(Type type)
        {
            Microsoft.Extensions.Logging.ILogger logger = _loggerFactory?.CreateLogger(type);
            if (logger == null)
            {
                return null;
            }
            return new Logger(logger);
        }
    }
}
