using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    /// <summary>
    /// 处理 multipart/form-data
    /// </summary>
    public class MultipartFormDataMediaTypeFormatter : IMediaTypeFormatter
    {
        public MultipartFormDataMediaTypeFormatter()
        {
            MediaType = Constants.MediaTypes.MULTIPART_FORMDATA;
        }
        public string MediaType { get; }

        public TResult GetResult<TResult>(byte[] buffer, Encoding encoding)
        {
            throw new NotSupportedException();
        }

    }
}
