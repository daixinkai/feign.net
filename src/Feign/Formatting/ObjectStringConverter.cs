using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// default convert object to string
    /// </summary>
    public sealed class ObjectStringConverter : IConverter<object, string>, IStringConverter<object>
    {
        public string? Convert(object? value)
        {
            return value?.ToString();
        }
    }

    internal class ClassToStringConverter<T> : IConverter<T, string>, IStringConverter<T> where T : class
    {
        public string? Convert(T? value)
        {
            return value?.ToString();
        }
    }

    internal class StructToStringConverter<T> : IConverter<T, string>, IStringConverter<T> where T : struct
    {
        public string? Convert(T value)
        {
            return value.ToString();
        }
    }

    internal class BooleanToStringConverter : IConverter<bool, string>, IStringConverter<bool>
    {
        public string? Convert(bool value)
            => value ? "true" : "false";
    }

    internal class StringToStringConverter : IConverter<string, string>, IStringConverter<string>
    {
        public string? Convert(string? value) => value;
    }

    internal class DateTimeToStringConverter : IConverter<DateTime, string>, IStringConverter<DateTime>
    {
        public string? Convert(DateTime value)
            => value.ToString();
    }

    internal class DateTimeOffsetToStringConverter : IConverter<DateTimeOffset, string>, IStringConverter<DateTimeOffset>
    {
        public string? Convert(DateTimeOffset value)
            => value.ToString("yyyy-MM-ddTHH:mm:ssK", DateTimeFormatInfo.InvariantInfo);
    }

#if NET6_0_OR_GREATER
    internal class DateOnlyToStringConverter : IConverter<DateOnly, string>, IStringConverter<DateOnly>
    {
        public string? Convert(DateOnly value)
            => value.ToString("yyyy-MM-dd");
    }
    internal class TimeOnlyToStringConverter : IConverter<TimeOnly, string>, IStringConverter<TimeOnly>
    {
        public string? Convert(TimeOnly value)
            => value.ToString("HH:mm:ss");
    }
#endif

}
