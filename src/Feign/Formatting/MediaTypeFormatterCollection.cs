using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    /// <summary>
    /// Media formatter collection
    /// </summary>
    public sealed class MediaTypeFormatterCollection : IEnumerable<IMediaTypeFormatter>
    {

        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, IMediaTypeFormatter> _map = new();

#if NET8_0_OR_GREATER
        private FrozenDictionary<string, IMediaTypeFormatter> _frozenMap = FrozenDictionary<string, IMediaTypeFormatter>.Empty;
        private void SyncFrozenMap() => _frozenMap = _map.ToFrozenDictionary();
#endif


        public IEnumerator<IMediaTypeFormatter> GetEnumerator()
        {
            return _map.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a media formatter
        /// </summary>
        /// <param name="formatter"></param>
        public void AddFormatter(IMediaTypeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            if (_map.ContainsKey(formatter.MediaType))
            {
                _map[formatter.MediaType] = formatter;
            }
            else
            {
                _map.TryAdd(formatter.MediaType, formatter);
            }
#if NET8_0_OR_GREATER
            SyncFrozenMap();
#endif
        }
        /// <summary>
        /// Find the specified media formatter
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public IMediaTypeFormatter? FindFormatter(string? mediaType)
        {
            if (mediaType == null)
            {
                return null;
            }
#if NET8_0_OR_GREATER
            _frozenMap.TryGetValue(mediaType, out var formatter);
#else
            _map.TryGetValue(mediaType, out var formatter);
#endif
            return formatter;
        }

    }
}
