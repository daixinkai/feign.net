using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    /// <summary>
    /// 一个接口,表示请求映射
    /// </summary>
    internal interface IRequestMapping
    {
        string GetMethod();
    }
}
