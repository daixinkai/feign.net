using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// An interface that supports converting objects
    /// </summary>
    public interface IStringConverter
    {
    }
    /// <summary>
    /// An interface that supports converting objects
    /// </summary>
    public interface IStringConverter<in TSource> : IStringConverter
    {
        string? Convert(TSource? value);
    }
}
