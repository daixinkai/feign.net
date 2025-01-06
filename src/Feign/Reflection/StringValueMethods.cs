using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class StringValueMethods
    {
        public static MethodInfo? GetToStringMethod(Type type)
        {
            if (type.IsNullableType() && type.GenericTypeArguments[0].IsPrimitive)
            {
                return typeof(StringValueMethods).GetMethods()
                    .FirstOrDefault(static s => s.Name == "NullableToString" && s.IsGenericMethod)?.MakeGenericMethod(type.GetGenericArguments()[0]);
            }
            return typeof(StringValueMethods).GetMethod("ToString", new Type[] { type });
        }

        public static string ToString(string value)
        {
            return value;
        }

        public static string ToString(int value)
        {
            return value.ToString();
        }

        public static string ToString(long value)
        {
            return value.ToString();
        }
        public static string ToString(byte value)
        {
            return value.ToString();
        }
        public static string ToString(bool value)
        {
            return value.ToString();
        }
        public static string ToString(decimal value)
        {
            return value.ToString();
        }
        public static string ToString(double value)
        {
            return value.ToString();
        }
        public static string? NullableToString<T>(T? value) where T : struct
        {
            if (!value.HasValue)
            {
                return null;
            }
            return value.Value.ToString();
        }

    }
}
