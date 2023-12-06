using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

#if NET45
using DictionaryKey = System.Tuple<System.Type, System.Type>;
using ConvertDictionaryKey = System.Tuple<System.Type, System.Type, System.Type, System.Type>;
#else
using DictionaryKey = System.ValueTuple<System.Type, System.Type>;
using ConvertDictionaryKey = System.ValueTuple<System.Type, System.Type, System.Type, System.Type>;
#endif

namespace Feign.Formatting
{
    /// <summary>
    /// Converter collection
    /// </summary>
    public sealed class ConverterCollection : IEnumerable<IConverter>
    {

        private readonly System.Collections.Concurrent.ConcurrentDictionary<DictionaryKey, IConverter> _map = new();

        private readonly System.Collections.Concurrent.ConcurrentDictionary<ConvertDictionaryKey, Delegate> _convertMap = new();

#if NET8_0_OR_GREATER
        private FrozenDictionary<DictionaryKey, IConverter> _frozenMap = FrozenDictionary<DictionaryKey, IConverter>.Empty;
        private void SyncFrozenMap() => _frozenMap = _map.ToFrozenDictionary();
#endif

        public IEnumerator<IConverter> GetEnumerator()
        {
            return _map.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a converter
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="converter"></param>
        public void AddConverter<TSource, TResult>(IConverter<TSource, TResult> converter)
        {
#if NET45
            var key = Tuple.Create(typeof(TSource), typeof(TResult));
#else
            var key = (typeof(TSource), typeof(TResult));
#endif
            if (_map.ContainsKey(key))
            {
                _map[key] = converter;
            }
            else
            {
                _map.TryAdd(key, converter);
            }
#if NET8_0_OR_GREATER
            SyncFrozenMap();
#endif
        }
        /// <summary>
        /// Find a converter for a specified type
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public IConverter<TSource, TResult>? FindConverter<TSource, TResult>()
        {
            IConverter? converter = FindConverter(typeof(TSource), typeof(TResult));
            return converter == null ? null : (IConverter<TSource, TResult>)converter;
        }
        /// <summary>
        /// Find a converter for a specified type
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="resultType"></param>
        /// <returns></returns>
        public IConverter? FindConverter(Type sourceType, Type resultType)
        {
#if NET45
            var key = Tuple.Create(sourceType, resultType);
#else
            var key = (sourceType, resultType);
#endif
#if NET8_0_OR_GREATER
            _frozenMap.TryGetValue(key, out var converter);
#else
            _map.TryGetValue(key, out var converter);
#endif
            return converter;
        }

        internal TResult? ConvertValue<TResult>(object? value, bool useDefault)
        {
            if (value == null)
            {
                return default;
            }
            var converter = FindConverter(value.GetType(), typeof(TResult));
            if (converter == null)
            {
                if (!useDefault)
                {
                    return default;
                }
                return ConvertDefaultValue<TResult>(value);
            }
            ////TODO : optimize
            //object? convertValue = converter.GetType().GetRequiredMethod("Convert").Invoke(converter, new[] { value });
            //if (convertValue == null)
            //{
            //    return default;
            //}
            //return (TResult)convertValue;
            return InvokeConvert<TResult>(converter, value);
        }

        internal TResult? ConvertValue<TSource, TResult>(TSource value, bool useDefault)
        {
            var converter = FindConverter<TSource, TResult>();
            if (converter == null)
            {
                if (!useDefault)
                {
                    return default;
                }
                return ConvertDefaultValue<TResult>(value);
            }
            return converter.Convert(value);
        }

        private TResult? ConvertDefaultValue<TResult>(object? value)
        {
            var converter = FindConverter<object, TResult>();
            if (converter == null)
            {
                return default;
            }
            return converter.Convert(value);
        }

        private TResult? InvokeConvert<TResult>(IConverter converter, object value)
        {
#if NET45
            var key = Tuple.Create(converter.GetType(), typeof(object), typeof(TResult), value.GetType());
#else
            var key = (converter.GetType(), typeof(object), typeof(TResult), value.GetType());
#endif
            var func = _convertMap.GetOrAdd(key, CreateConvertDelegate) as Func<IConverter, object, TResult?>;
            var result = func!.Invoke(converter, value);
            return result;
        }

        private Delegate CreateConvertDelegate(ConvertDictionaryKey key)
        {
            var convertType = key.Item1;
            var sourceType = key.Item2;
            var resultType = key.Item3;
            var valueType = key.Item4;
            var instance = Expression.Parameter(typeof(IConverter));
            var source = Expression.Parameter(sourceType);
            var methodParameter = source.Type == valueType ? (Expression)source : Expression.Convert(source, valueType);
            var method = convertType.GetRequiredMethod("Convert");
            var body = Expression.Call(Expression.Convert(instance, convertType), method, methodParameter);
            var delegateType = typeof(Func<,,>).MakeGenericType(new[] { typeof(IConverter), sourceType, resultType });
            return Expression.Lambda(delegateType, body, instance, source).Compile();
        }
    }
}
