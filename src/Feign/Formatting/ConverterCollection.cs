﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    /// <summary>
    /// Converter collection
    /// </summary>
    public sealed class ConverterCollection : IEnumerable<IConverter>
    {

#if NET45
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Tuple<Type, Type>, IConverter> _map = new ();
#else
        private readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), IConverter> _map = new();
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
            _map.TryGetValue(key, out var converter);
            return converter;
        }

        internal TResult? ConvertValue<TResult>(object value, bool useDefault)
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
            //TODO : optimize
            object? convertValue = converter.GetType().GetRequiredMethod("Convert").Invoke(converter, new[] { value });
            if (convertValue == null)
            {
                return default;
            }
            return (TResult)convertValue;
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

    }
}
