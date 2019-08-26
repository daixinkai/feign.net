using Feign.Discovery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Tests
{
    class TestServiceInstance : IServiceInstance
    {
        public TestServiceInstance(Uri uri)
        {
            Uri = uri;
        }
        public TestServiceInstance(string serviceId, Uri uri)
        {
            ServiceId = serviceId;
            Uri = uri;
        }

        public TestServiceInstance(string uri) : this(new Uri(uri))
        {
        }
        public TestServiceInstance(string serviceId, string uri) : this(serviceId, new Uri(uri))
        {
        }

        public string ServiceId { get; set; }

        public string Host => Uri.Host;

        public int Port => Uri.Port;

        public Uri Uri { get; }
    }
}
