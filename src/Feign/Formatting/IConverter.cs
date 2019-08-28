using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// 一个接口,支持转换对象
    /// </summary>
    public interface IConverter
    {
    }
    /// <summary>
    /// 一个接口,支持转换对象
    /// </summary>
    public interface IConverter<in TSource, out TResult> : IConverter
    {
        TResult Convert(TSource value);
    }
}
