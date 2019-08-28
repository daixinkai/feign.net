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
    /// 媒体处理器集合
    /// </summary>
    public sealed class MediaTypeFormatterCollection : IEnumerable<IMediaTypeFormatter>
    {

        System.Collections.Concurrent.ConcurrentDictionary<string, IMediaTypeFormatter> _map = new System.Collections.Concurrent.ConcurrentDictionary<string, IMediaTypeFormatter>();


        public IEnumerator<IMediaTypeFormatter> GetEnumerator()
        {
            return _map.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 添加一个媒体处理器
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
        /// 查找指定的媒体处理器
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public IMediaTypeFormatter FindFormatter(string mediaType)
        {
            if (mediaType == null)
            {
                return null;
            }
            IMediaTypeFormatter formatter;
            _map.TryGetValue(mediaType, out formatter);
            return formatter;
        }

    }
}
