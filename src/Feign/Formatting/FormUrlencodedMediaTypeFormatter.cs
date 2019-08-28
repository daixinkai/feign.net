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
    /// 处理 application/x-www-form-urlencoded 
    /// </summary>
    public class FormUrlEncodedMediaTypeFormatter : IMediaTypeFormatter
    {
        public FormUrlEncodedMediaTypeFormatter()
        {
            MediaType = Constants.MediaTypes.APPLICATION_FORM_URLENCODED;
        }
        public string MediaType { get; }

        public TResult GetResult<TResult>(byte[] buffer, Encoding encoding)
        {
            throw new NotSupportedException();
        }
    }
}
