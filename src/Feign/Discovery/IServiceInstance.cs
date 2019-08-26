using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    public interface IServiceInstance
    {
        string ServiceId { get; }
        string Host { get; }
        int Port { get; }
        Uri Uri { get; }
    }
}
