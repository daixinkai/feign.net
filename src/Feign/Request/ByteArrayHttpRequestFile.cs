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
    public class ByteArrayHttpRequestFile : IHttpRequestFile
    {
        public ByteArrayHttpRequestFile(byte[] buffer, string fileName)
        {
            Buffer = buffer;
            FileName = fileName;
        }

        public byte[] Buffer { get; }
        public string Name { get; set; }
        public string FileName { get; }
        public string MediaType { get; set; }

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
