using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request.Headers
{
    /// <summary>
    /// 设置请求头
    /// </summary>
    public interface IRequestHeaderHandler
    {
        void SetHeader(HttpRequestMessage httpRequestMessage);
    }
}
