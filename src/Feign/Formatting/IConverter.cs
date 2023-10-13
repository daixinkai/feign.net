using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// An interface that supports converting objects
    /// </summary>
    public interface IConverter
    {
    }
    /// <summary>
    /// An interface that supports converting objects
    /// </summary>
    public interface IConverter<in TSource, out TResult> : IConverter
    {
        TResult? Convert(TSource? value);
    }
}
