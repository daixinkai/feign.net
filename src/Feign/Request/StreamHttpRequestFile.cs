using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 文件流请求文件
    /// </summary>
    public class StreamHttpRequestFile : IHttpRequestFile, IMultipartFormData
    {
        public StreamHttpRequestFile(Stream stream, string fileName)
        {
            Stream = stream;
            FileName = fileName;
        }

        public Stream Stream { get; }
        public string? Name { get; set; }
        public string FileName { get; }
        public string? MediaType { get; set; }

        /// <summary>
        /// <para>true : Content-Type = multipart/form-data; boundary="123456789"</para> 
        /// <para>false : Content-Type = multipart/form-data; boundary=123456789</para> 
        /// <para>default is true</para> 
        /// </summary>
        public bool QuotedBoundary { get; set; } = true;

        HttpContent IHttpRequestFile.GetHttpContent()
        {
            StreamContent streamContent = new StreamContent(Stream);
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue(Constants.MediaTypes.FORMDATA)
            {
                FileName = FileName,
                Name = Name
            };
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaType ?? Constants.MediaTypes.APPLICATION_STREAM);
            return streamContent;
        }
    }
}
