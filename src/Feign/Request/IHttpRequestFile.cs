using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 一个接口,表示一个请求文件
    /// </summary>
    public interface IHttpRequestFile
    {
        /// <summary>
        /// 获取此文件的name
        /// </summary>
        string? Name { get; }
        /// <summary>
        /// 获取随请求一起传输的HttpContent
        /// </summary>
        /// <returns></returns>
        HttpContent GetHttpContent();
    }
}
