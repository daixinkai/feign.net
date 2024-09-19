using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Feign.Logging
{
    internal class LoggerFactory : ILoggerFactory
    {
        public LoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory? loggerFactory = null)
        {
            _loggerFactory = loggerFactory;
        }

        private readonly Microsoft.Extensions.Logging.ILoggerFactory? _loggerFactory;

        public ILogger CreateLogger(Type type)
        {
            var logger = _loggerFactory?.CreateLogger(type);
            if (logger == null)
            {
                return new DefaultLogger();
            }
            return new Logger(logger);
        }
    }
}
