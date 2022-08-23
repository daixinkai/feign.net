using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    public interface IMultipartFormData
    {
        /// <summary>
        /// <para>true : Content-Type = multipart/form-data; boundary="123456789"</para> 
        /// <para>false : Content-Type = multipart/form-data; boundary=123456789</para> 
        /// </summary>
        bool QuotedBoundary { get; }
    }
}
