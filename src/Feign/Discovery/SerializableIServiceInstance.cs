using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    [Serializable]
    public class SerializableIServiceInstance : IServiceInstance
    {
        public SerializableIServiceInstance(IServiceInstance instance)
        {
            ServiceId = instance.ServiceId;
            Host = instance.Host;
            Port = instance.Port;
            //IsSecure = instance.IsSecure;
            Uri = instance.Uri;
            //Metadata = instance.Metadata;
        }

        public string ServiceId { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        //public bool IsSecure { get; set; }

        public Uri Uri { get; set; }

        //public IDictionary<string, string> Metadata { get; set; }
    }
}
