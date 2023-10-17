using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// An interface representing a request file
    /// </summary>
    public interface IHttpRequestFile
    {
        /// <summary>
        /// Gets the name of request file
        /// </summary>
        string? Name { get; }
        /// <summary>
        /// Get the HttpContent with the request
        /// </summary>
        /// <returns></returns>
        HttpContent GetHttpContent();
    }
}
