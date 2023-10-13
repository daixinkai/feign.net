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

        private System.Collections.Concurrent.ConcurrentDictionary<string, IMediaTypeFormatter> _map = new System.Collections.Concurrent.ConcurrentDictionary<string, IMediaTypeFormatter>();


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
            _map.TryGetValue(mediaType, out var formatter);
            return formatter;
        }

    }
}
