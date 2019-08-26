using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Feign.Formatting
{
    public sealed class ConverterCollection : IEnumerable<IConverter>
    {

#if NETSTANDARD
        System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), IConverter> _map = new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), IConverter>();
#endif

#if NET45
        System.Collections.Concurrent.ConcurrentDictionary<Tuple<Type, Type>, IConverter> _map = new System.Collections.Concurrent.ConcurrentDictionary<Tuple<Type, Type>, IConverter>();
#endif


        public IEnumerator<IConverter> GetEnumerator()
        {
            return _map.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void AddConverter<TSource, TResult>(IConverter<TSource, TResult> converter)
        {
#if NETSTANDARD
            var key = (typeof(TSource), typeof(TResult));
#endif
#if NET45
            var key = Tuple.Create(typeof(TSource), typeof(TResult));
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

        public IConverter<TSource, TResult> FindConverter<TSource, TResult>()
        {
            IConverter converter = FindConverter(typeof(TSource), typeof(TResult));
            return converter == null ? null : (IConverter<TSource, TResult>)converter;
        }

        public IConverter FindConverter(Type sourceType, Type resultType)
        {
#if NETSTANDARD
            var key = (sourceType, resultType);
#endif
#if NET45
            var key = Tuple.Create(sourceType, resultType);
#endif
            IConverter converter;
            _map.TryGetValue(key, out converter);
            return converter;
        }

        internal TResult ConvertValue<TResult>(object value, bool useDefault)
        {
            if (value == null)
            {
                return default(TResult);
            }
            var converter = FindConverter(value.GetType(), typeof(TResult));
            if (converter == null)
            {
                if (!useDefault)
                {
                    return default(TResult);
                }
                return FindConverter<object, TResult>().Convert(value);
            }
            //TODO : optimize
            object convertValue = converter.GetType().GetMethod("Convert").Invoke(converter, new[] { value });
            if (convertValue == null)
            {
                return default(TResult);
            }
            return (TResult)convertValue;
        }

        internal TResult ConvertValue<TSource, TResult>(TSource value, bool useDefault)
        {
            var converter = FindConverter<TSource, TResult>();
            if (converter == null)
            {
                if (!useDefault)
                {
                    return default(TResult);
                }
                return FindConverter<object, TResult>().Convert(value);
            }
            return converter.Convert(value);
        }

    }
}
