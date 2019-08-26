using Feign.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Tests
{
    public class TestServiceUploadFileParam : TestServiceParam, IHttpRequestFileForm
    {
        public IHttpRequestFile File { get; set; }
        IEnumerable<IHttpRequestFile> IHttpRequestFileForm.RequestFiles => new[] { File };
    }
}
