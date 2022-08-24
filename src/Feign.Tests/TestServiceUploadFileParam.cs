using Feign.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Tests
{
    public class TestServiceUploadFileParam : TestServiceParam, IHttpRequestFileForm
    {
        public IHttpRequestFile File { get; set; }
        /// <summary>
        /// <para>true : Content-Type = multipart/form-data; boundary="123456789"</para> 
        /// <para>false : Content-Type = multipart/form-data; boundary=123456789</para> 
        /// <para>default is true</para> 
        /// </summary>
        public bool QuotedBoundary { get; set; } = true;
        IEnumerable<IHttpRequestFile> IHttpRequestFileForm.RequestFiles => new[] { File };

        public IEnumerable<KeyValuePair<string, string>> GetRequestForm() => null;
    }
}
