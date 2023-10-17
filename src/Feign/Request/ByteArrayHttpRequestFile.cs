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
    /// byte[] request file
    /// </summary>
    public class ByteArrayHttpRequestFile : IHttpRequestFile, IMultipartFormData
    {
        public ByteArrayHttpRequestFile(byte[] buffer, string fileName)
        {
            Buffer = buffer;
            FileName = fileName;
        }

        public byte[] Buffer { get; }
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
            ByteArrayContent byteArrayContent = new ByteArrayContent(Buffer);
            byteArrayContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = FileName,
                Name = Name
            };
            byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaType ?? "application/octet-stream");
            return byteArrayContent;
        }

    }
}
