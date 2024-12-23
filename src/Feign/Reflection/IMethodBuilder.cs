using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Feign.Reflection
{
    /// <summary>
    /// 方法生成器
    /// </summary>
    public interface IMethodBuilder
    {
        FeignClientMethodInfo BuildMethod(TypeBuilder typeBuilder, Type serviceType, MethodInfo method, FeignClientAttribute feignClientAttribute);
    }
}
