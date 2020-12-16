using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// 可序列化的服务
    /// </summary>
    [Serializable]
    public class SerializableServiceInstance : IServiceInstance
    {
        public SerializableServiceInstance(IServiceInstance instance)
        {
            ServiceId = instance.ServiceId;
            Host = instance.Host;
            Port = instance.Port;
            //IsSecure = instance.IsSecure;
            Uri = instance.Uri.ToString();
            //Metadata = instance.Metadata;
        }

        public string ServiceId { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        //public bool IsSecure { get; set; }

        public string Uri { get; set; }

        Uri IServiceInstance.Uri
        {
            get
            {
                return new Uri(Uri);
            }
        }

        //public IDictionary<string, string> Metadata { get; set; }
    }
}
