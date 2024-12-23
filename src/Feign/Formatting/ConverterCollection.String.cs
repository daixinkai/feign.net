using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#if NET45
using StringConvertDictionaryKey = System.Tuple<System.Type, System.Type,  System.Type>;
using ValueTuple=System.Tuple;
#else
using StringConvertDictionaryKey = System.ValueTuple<System.Type, System.Type, System.Type>;
#endif

namespace Feign.Formatting
{
    partial class ConverterCollection : IEnumerable<IStringConverter>
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Type, IStringConverter> _stringMap = new();

        private readonly System.Collections.Concurrent.ConcurrentDictionary<StringConvertDictionaryKey, Delegate> _stringConvertMap = new();

#if NET8_0_OR_GREATER
        private FrozenDictionary<Type, IStringConverter> _stringFrozenMap = FrozenDictionary<Type, IStringConverter>.Empty;
        private void SyncStringFrozenMap() => _stringFrozenMap = _stringMap.ToFrozenDictionary();
#endif

        IEnumerator<IStringConverter> IEnumerable<IStringConverter>.GetEnumerator()
        {
            return _stringMap.Values.GetEnumerator();
        }

        /// <summary>
        /// Add a string converter
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="key"></param>
        /// <param name="completed"></param>
        internal void AddStringConverter(IStringConverter converter, Type key, bool completed)
        {
            if (_stringMap.ContainsKey(key))
            {
                _stringMap[key] = converter;
            }
            else
            {
                _stringMap.TryAdd(key, converter);
            }
#if NET8_0_OR_GREATER
            if (completed)
            {
                SyncStringFrozenMap();
            }
#endif
        }

        internal void RemoveStringConverter(Type key, bool completed)
        {
            if (_stringMap.TryRemove(key, out var converter))
            {
#if NET8_0_OR_GREATER
                if (completed)
                {
                    SyncStringFrozenMap();
                }
#endif
            }
        }


        /// <summary>
        /// Find a string converter for a specified type
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public IStringConverter<TSource>? FindStringConverter<TSource>()
        {
            IStringConverter? converter = FindStringConverter(typeof(TSource));
            return converter == null ? null : (IStringConverter<TSource>)converter;
        }

        /// <summary>
        /// Find a string converter for a specified type
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public IStringConverter? FindStringConverter(Type sourceType)
        {
#if NET8_0_OR_GREATER
            _stringFrozenMap.TryGetValue(sourceType, out var converter);
#else
            _stringMap.TryGetValue(sourceType, out var converter);
#endif
            return converter;
        }

        internal bool TryConvertStringValue<TSource>(TSource? value, out string? result)
        {
            result = default;
            if (value == null)
            {
                return false;
            }
            var converter = FindStringConverter(value.GetType());
            if (converter == null)
            {
                return TryConvertValue(value, out result);
            }
            result = InvokeStringConvert(converter, value);
            return true;
        }

        internal string? ConvertStringValue<TSource>(TSource? value, bool useDefault)
        {
            if (TryConvertStringValue(value, out var result))
            {
                return result;
            }
            if (useDefault)
            {
                return ConvertDefaultStringValue(value);
            }
            return default;
        }

        /// <summary>
        ///  object -> TResult
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string? ConvertDefaultStringValue(object? value)
        {
            var converter = FindStringConverter<object>();
            if (converter == null)
            {
                return ConvertDefaultValue<string>(value);
            }
            return converter.Convert(value);
        }

        private string? InvokeStringConvert<TSource>(IStringConverter converter, TSource value)
        {
            var key = ValueTuple.Create(converter.GetType(), typeof(TSource), value!.GetType());
            var func = _stringConvertMap.GetOrAdd(key, CreateStringConvertDelegate) as Func<IStringConverter, TSource, string?>;
            var result = func!.Invoke(converter, value);
            return result;
        }

        private Delegate CreateStringConvertDelegate(StringConvertDictionaryKey key)
        {
            var convertType = key.Item1;
            var sourceType = key.Item2;
            var valueType = key.Item3;
            var instance = Expression.Parameter(typeof(IStringConverter));
            var source = Expression.Parameter(sourceType);
            var methodParameter = source.Type == valueType ? (Expression)source : Expression.Convert(source, valueType);
            var method = typeof(IStringConverter<>).MakeGenericType(valueType).GetRequiredMethod("Convert");
            var body = Expression.Call(Expression.Convert(instance, convertType), method, methodParameter);
            var delegateType = typeof(Func<,,>).MakeGenericType(new[] { typeof(IStringConverter), sourceType, typeof(string) });
            return Expression.Lambda(delegateType, body, instance, source).Compile();
        }

    }
}
