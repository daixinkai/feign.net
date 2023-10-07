using Feign.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    internal partial class ObjectQueryMap<T>
    {

        public ObjectQueryMap(T value, NamingPolicy namingPolicy, ConverterCollection converters)
        {
            Value = value;
            NamingPolicy = namingPolicy;
            Converters = converters;
        }

        public T Value { get; }

        public NamingPolicy NamingPolicy { get; }

        public ConverterCollection Converters { get; }


        public IEnumerable<KeyValuePair<string, string?>> GetStringParameters()
        {

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetMethod == null)
                {
                    continue;
                }
                object? propertyValue = property.GetValue(Value);
                if (propertyValue == null)
                {
                    continue;
                }
                if (propertyValue is string)
                {
                    yield return new KeyValuePair<string, string?>(GetName(property, NamingPolicy), propertyValue.ToString());
                    continue;
                }
                if (propertyValue is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        yield return new KeyValuePair<string, string?>(GetName(property, NamingPolicy), Converters.ConvertValue<string>(item, true));
                    }
                    continue;
                }
                yield return new KeyValuePair<string, string?>(GetName(property, NamingPolicy), Converters.ConvertValue<string>(propertyValue, true));
            }
        }


        private static string GetName(PropertyInfo property, NamingPolicy namingPolicy)
        {
            return namingPolicy.ConvertName(property.Name);
        }


    }
}
