using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// http Form
    /// </summary>
    public interface IHttpRequestForm
    {
        IEnumerable<KeyValuePair<string, string?>> GetRequestForm();
    }
}
