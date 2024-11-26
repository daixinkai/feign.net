using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// default convert object to string
    /// </summary>
    public sealed class ObjectStringConverter : IConverter<object, string>
    {
        public string? Convert(object? value)
        {
            return value?.ToString();
        }
    }

    internal class ClassToStringConverter<T> : IConverter<T, string> where T : class
    {
        public string? Convert(T? value)
        {
            return value?.ToString();
        }
    }

    internal class StructToStringConverter<T> : IConverter<T, string> where T : struct
    {
        public string? Convert(T value)
        {
            return value.ToString();
        }
    }

    internal class BooleanToStringConverter : IConverter<bool, string>
    {
        public string? Convert(bool value)
            => value ? "true" : "false";
    }

    internal class StringToStringConverter : IConverter<string, string>
    {
        public string? Convert(string? value) => value;
    }

    internal class DateTimeToStringConverter : IConverter<DateTime, string>
    {
        public string? Convert(DateTime value)
            => value.ToString();
    }

    internal class DateTimeOffsetToStringConverter : IConverter<DateTimeOffset, string>
    {
        public string? Convert(DateTimeOffset value)
            => value.ToString("yyyy-MM-ddTHH:mm:ssK", DateTimeFormatInfo.InvariantInfo);
    }

}
