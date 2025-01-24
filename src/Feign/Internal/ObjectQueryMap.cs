using Feign.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Feign.Internal
{
    internal partial class ObjectQueryMap<T>
    {

        private struct DictionaryParameters
        {
            public DictionaryParameters(
                string? prefix,
                string? name,
                IDictionary dictionary,
                NamingPolicy namingPolicy,
                ConverterCollection converters)
            {
                _prefix = prefix;
                _name = name;
                _dictionary = dictionary;
                _namingPolicy = namingPolicy;
                _converters = converters;
            }

            private readonly string? _prefix;
            private readonly string? _name;
            private readonly IDictionary _dictionary;
            private readonly NamingPolicy _namingPolicy;
            private readonly ConverterCollection _converters;


            public IEnumerable<KeyValuePair<string, string?>> GetStringParameters()
            {
                string prefix = GetPrefix(_prefix, _name, _namingPolicy);
                foreach (var key in _dictionary.Keys)
                {
                    var dictionaryValue = _dictionary[key!];
                    if (dictionaryValue == null)
                    {
                        continue;
                    }
                    string keyName = key?.ToString() ?? "undefind";
                    if (dictionaryValue is string)
                    {
                        yield return CreateParameter(prefix, keyName, dictionaryValue.ToString(), NamingPolicy.Default);
                        continue;
                    }

                    var knownParameters = GetKnownParameters(prefix, keyName, dictionaryValue, _namingPolicy, _converters);

                    if (knownParameters != null)
                    {
                        foreach (var item in knownParameters)
                        {
                            yield return item;
                        }
                        continue;
                    }

                    if (_converters.TryConvertStringValue(dictionaryValue, out var result))
                    {
                        yield return CreateParameter(prefix, keyName, result, NamingPolicy.Default);
                        continue;
                    }

                    foreach (var item in new UnknownParameters(prefix, keyName, dictionaryValue, _namingPolicy, _converters, false).GetStringParameters())
                    {
                        yield return item;
                    }

                }
            }

        }

        private struct EnumerableParameters
        {
            public EnumerableParameters(
                string? prefix,
                string name,
                IEnumerable enumerable,
                NamingPolicy namingPolicy,
                ConverterCollection converters)
            {
                _prefix = prefix;
                _name = name;
                _enumerable = enumerable;
                _namingPolicy = namingPolicy;
                _converters = converters;
            }

            private readonly string? _prefix;
            private readonly string _name;
            private readonly IEnumerable _enumerable;
            private readonly NamingPolicy _namingPolicy;
            private readonly ConverterCollection _converters;


            public IEnumerable<KeyValuePair<string, string?>> GetStringParameters()
            {
                foreach (var item in _enumerable)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (_converters.TryConvertStringValue(item, out var result))
                    {
                        yield return CreateParameter(_prefix, _name, result, _namingPolicy);
                    }
                    else
                    {
                        foreach (var itemParameters in new UnknownParameters(_prefix, _name, item, _namingPolicy, _converters, true).GetStringParameters())
                        {
                            yield return itemParameters;
                        }
                    }
                }
            }

        }

        private struct ObjectParameters
        {
            public ObjectParameters(
                string? prefix,
                string? name,
                object value,
                NamingPolicy namingPolicy,
                ConverterCollection converters)
            {
                _prefix = prefix;
                _name = name;
                _value = value;
                _namingPolicy = namingPolicy;
                _converters = converters;
            }

            private readonly string? _prefix;
            private readonly string? _name;
            private readonly object _value;
            private readonly NamingPolicy _namingPolicy;
            private readonly ConverterCollection _converters;


            public IEnumerable<KeyValuePair<string, string?>> GetStringParameters()
            {
                foreach (var property in _value.GetType().GetProperties())
                {
                    if (property.GetMethod == null)
                    {
                        continue;
                    }
                    object? propertyValue = property.GetValue(_value);
                    if (propertyValue == null)
                    {
                        continue;
                    }

                    //switch (Type.GetTypeCode(property.PropertyType))
                    //{
                    //    case TypeCode.Empty:
                    //        break;
                    //    case TypeCode.Object:
                    //        break;
                    //    case TypeCode.DBNull:
                    //        break;
                    //    case TypeCode.Boolean:
                    //        break;
                    //    case TypeCode.Char:
                    //        break;
                    //    case TypeCode.SByte:
                    //        break;
                    //    case TypeCode.Byte:
                    //        break;
                    //    case TypeCode.Int16:
                    //        break;
                    //    case TypeCode.UInt16:
                    //        break;
                    //    case TypeCode.Int32:
                    //        break;
                    //    case TypeCode.UInt32:
                    //        break;
                    //    case TypeCode.Int64:
                    //        break;
                    //    case TypeCode.UInt64:
                    //        break;
                    //    case TypeCode.Single:
                    //        break;
                    //    case TypeCode.Double:
                    //        break;
                    //    case TypeCode.Decimal:
                    //        break;
                    //    case TypeCode.DateTime:
                    //        break;
                    //    case TypeCode.String:
                    //        break;
                    //    default:
                    //        break;
                    //}

                    if (propertyValue is string)
                    {
                        yield return CreateParameter(_prefix, property.Name, propertyValue.ToString(), _namingPolicy);
                        continue;
                    }

                    var knownParameters = GetKnownParameters(_prefix, property.Name, propertyValue, _namingPolicy, _converters);
                    if (knownParameters != null)
                    {
                        foreach (var item in knownParameters)
                        {
                            yield return item;
                        }
                        continue;
                    }

                    if (_converters.TryConvertStringValue(propertyValue, out var result))
                    {
                        yield return CreateParameter(_prefix, property.Name, result, _namingPolicy);
                        continue;
                    }

                    if (propertyValue.GetType().IsValueType)
                    {
                        yield return CreateParameter(_prefix, property.Name, propertyValue.ToString(), _namingPolicy);
                        continue;
                    }

                    foreach (var item in new UnknownParameters(_prefix, property.Name, propertyValue, _namingPolicy, _converters, true).GetStringParameters())
                    {
                        yield return item;
                    }

                }
            }

        }

        private struct UnknownParameters
        {
            public UnknownParameters(
                string? prefix,
                string name,
                object value,
                NamingPolicy namingPolicy,
                ConverterCollection converters,
                bool useSimpleNamingPolicy)
            {
                _prefix = prefix;
                _name = name;
                _value = value;
                _namingPolicy = namingPolicy;
                _converters = converters;
                _useSimpleNamingPolicy = useSimpleNamingPolicy;
            }

            private readonly string? _prefix;
            private readonly string _name;
            private readonly object _value;
            private readonly NamingPolicy _namingPolicy;
            private readonly ConverterCollection _converters;
            private readonly bool _useSimpleNamingPolicy;

            public IEnumerable<KeyValuePair<string, string?>> GetStringParameters()
            {
                var simpleNamingPolicy = _useSimpleNamingPolicy ? _namingPolicy : NamingPolicy.Default;
                var type = _value.GetType();
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Empty:
                        yield break;
                    case TypeCode.Object:
                        var specialValue = GetSpecialObjectValue(type, _value);
                        if (specialValue != null)
                        {
                            yield return new KeyValuePair<string, string?>(GetPrefix(_prefix, _name, simpleNamingPolicy), specialValue);
                        }
                        else
                        {
                            foreach (var item in new ObjectParameters(GetPrefix(_prefix, _name, simpleNamingPolicy), _name, _value, _namingPolicy, _converters).GetStringParameters())
                            {
                                yield return item;
                            }
                        }
                        break;
                    case TypeCode.String:
                        yield return CreateParameter(_prefix, _name, _value.ToString(), simpleNamingPolicy);
                        break;
                    case TypeCode.DBNull:
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                    default:
                        yield return CreateParameter(_prefix, _name, _converters.ConvertStringValue(_value, true), simpleNamingPolicy);
                        break;
                }
            }

            private string? GetSpecialObjectValue(Type type, object value)
            {
                return _converters.ConvertStringValue(value, false);
            }

        }

        public ObjectQueryMap(string name, T value, NamingPolicy namingPolicy, ConverterCollection converters)
        {
            Name = name;
            Value = value;
            NamingPolicy = namingPolicy;
            Converters = converters;
        }

        public string Name { get; }

        public T Value { get; }

        public NamingPolicy NamingPolicy { get; }

        public ConverterCollection Converters { get; }

        public IEnumerable<KeyValuePair<string, string?>> GetStringParameters(bool includeRootName)
        {
            var type = Value!.GetType();
            //if (typeof(IDictionary).IsAssignableFrom(type))
            if (Value is IDictionary dictionary)
            {
                if (includeRootName)
                {
                    return new DictionaryParameters(null, Name, dictionary, NamingPolicy, Converters).GetStringParameters();
                }
                return new DictionaryParameters(null, null, dictionary, NamingPolicy, Converters).GetStringParameters();
            }
            //if (typeof(IEnumerable).IsAssignableFrom(type))
            if (Value is IEnumerable enumerable)
            {
                return new EnumerableParameters(null, Name, enumerable, NamingPolicy, Converters).GetStringParameters();
            }
            if (includeRootName)
            {
                return new UnknownParameters(null, Name, Value!, NamingPolicy, Converters, true).GetStringParameters();
            }
            return new UnknownParameters(null, "", Value!, NamingPolicy, Converters, true).GetStringParameters();
        }

        private static IEnumerable<KeyValuePair<string, string?>>? GetKnownParameters(string? prefix, string name, object value, NamingPolicy namingPolicy, ConverterCollection converters)
        {
            if (value is IDictionary dictionary)
            {
                return new DictionaryParameters(prefix, name, dictionary, namingPolicy, converters).GetStringParameters();
            }

            if (value is IEnumerable enumerable)
            {
                return new EnumerableParameters(prefix, name, enumerable, namingPolicy, converters).GetStringParameters();
            }
            //if (converters.TryConvertValue<string>(value, out var result))
            //{
            //    yield return CreateParameter(prefix, name, result, NamingPolicy.Default);
            //}
            return null;
        }

        private static KeyValuePair<string, string?> CreateParameter(string? prefix, string name, string? value, NamingPolicy namingPolicy)
            => new KeyValuePair<string, string?>(GetName(prefix, name, namingPolicy), value);

        private static string GetName(string? prefix, string? name, NamingPolicy namingPolicy)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }
            if (prefix == null)
            {
                return namingPolicy.ConvertName(name);
            }
            return prefix + namingPolicy.ConvertName(name);
        }

        private static string GetPrefix(string? prefix, string? name, NamingPolicy namingPolicy)
        {
            string value = GetName(prefix, name, namingPolicy);
            return string.IsNullOrWhiteSpace(value) ? "" : value + ".";
        }

    }
}