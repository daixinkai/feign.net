using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Logging
{
    public interface ILogger
    {
        void LogTrace(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogError(string message, params object[] args);

        void LogError(Exception exception, string message, params object[] args);

    }
}
