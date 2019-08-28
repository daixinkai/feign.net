using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示BuildingRequest阶段的事件参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IBuildingRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        /// <summary>
        /// 获取http method
        /// </summary>
        string Method { get; }
        /// <summary>
        /// 获取或设置请求路径
        /// </summary>
        Uri RequestUri { get; set; }
        /// <summary>
        /// 获取请求头
        /// </summary>
        IDictionary<string, string> Headers { get; }
    }
}
