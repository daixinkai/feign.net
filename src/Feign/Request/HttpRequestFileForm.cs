using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            RequestFiles = new[] { requestFile };
        }

        public HttpRequestFileForm(IEnumerable<IHttpRequestFile> requestFiles)
        {
            RequestFiles = requestFiles;
        }

        private Dictionary<string, string> _form = new Dictionary<string, string>();

        public IEnumerable<IHttpRequestFile> RequestFiles { get; set; }
        /// <summary>
        /// <para>true : Content-Type = multipart/form-data; boundary="123456789"</para> 
        /// <para>false : Content-Type = multipart/form-data; boundary=123456789</para> 
        /// <para>default is true</para> 
        /// </summary>
        public bool QuotedBoundary { get; set; } = true;

        public void AddForm(string name, string value)
        {
            _form[name] = value;
        }

        public IEnumerable<KeyValuePair<string, string>> GetRequestForm() => _form;
    }
}
