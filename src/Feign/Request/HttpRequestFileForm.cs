using System.Collections.Generic;

namespace Feign.Request
{
    /// <summary>
    /// 文件表单请求
    /// </summary>
    public class HttpRequestFileForm : IHttpRequestFileForm
    {
        public HttpRequestFileForm() { }

        public HttpRequestFileForm(IHttpRequestFile requestFile)
        {
            RequestFiles = new List<IHttpRequestFile>() { requestFile };
        }

        public HttpRequestFileForm(IEnumerable<IHttpRequestFile> requestFiles)
        {
            RequestFiles = requestFiles;
        }

        public IEnumerable<IHttpRequestFile> RequestFiles { get; set; }
    }
}
