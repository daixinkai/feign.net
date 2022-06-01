using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Feign.Logging
{
    internal class LoggerFactory : Feign.Logging.ILoggerFactory
    {
        public LoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        private readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

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
