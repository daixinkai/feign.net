using Steeltoe.Common.Discovery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    public class SteeltoeServiceInstance : IServiceInstance
    {
        public SteeltoeServiceInstance(Steeltoe.Common.Discovery.IServiceInstance serviceInstance)
        {
            _serviceInstance = serviceInstance;
        }

        Steeltoe.Common.Discovery.IServiceInstance _serviceInstance;

        public string ServiceId => _serviceInstance.ServiceId;

        public string Host => _serviceInstance.Host;

        public int Port => _serviceInstance.Port;

        public Uri Uri => _serviceInstance.Uri;


    }
}
