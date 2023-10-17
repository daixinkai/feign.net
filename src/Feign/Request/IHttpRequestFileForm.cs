using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// An interface that represents a form containing multiple request files
    /// </summary>
    public interface IHttpRequestFileForm : IHttpRequestForm, IMultipartFormData
    {
        /// <summary>
        /// Gets request file collection
        /// </summary>
        IEnumerable<IHttpRequestFile>? RequestFiles { get; }
    }
}
