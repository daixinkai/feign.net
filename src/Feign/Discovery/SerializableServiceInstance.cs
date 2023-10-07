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
        public string ServiceId { get; set; } = null!;

        public string Host { get; set; } = null!;

        public int Port { get; set; }

        //public bool IsSecure { get; set; }

        public string Uri { get; set; } = null!;

        Uri IServiceInstance.Uri
        {
            get
            {
                return new Uri(Uri);
            }
        }

        public static SerializableServiceInstance FromServiceInstance(IServiceInstance instance)
        {
            return new SerializableServiceInstance
            {
                ServiceId = instance.ServiceId,
                Host = instance.Host,
                Port = instance.Port,
                //IsSecure = instance.IsSecure;
                Uri = instance.Uri.ToString(),
                //Metadata = instance.Metadata;
            };
        }

        //public IDictionary<string, string> Metadata { get; set; }
    }
}
