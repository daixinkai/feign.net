using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 一个接口,表示含有多个请求文件表单
    /// </summary>
    public interface IHttpRequestFileForm
    {
        /// <summary>
        /// 获取请求文件
        /// </summary>
        IEnumerable<IHttpRequestFile> RequestFiles { get; }
    }
}
