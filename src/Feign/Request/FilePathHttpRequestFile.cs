using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 文件路径请求文件
    /// </summary>
    public class FilePathHttpRequestFile : StreamHttpRequestFile
    {
        public FilePathHttpRequestFile(string filePath) : base(File.OpenRead(filePath), Path.GetFileName(filePath))
        {
        }
    }
}
