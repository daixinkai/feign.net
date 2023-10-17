using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// FilePath request file
    /// </summary>
    public class FilePathHttpRequestFile : StreamHttpRequestFile
    {
        public FilePathHttpRequestFile(string filePath) : base(File.OpenRead(filePath), Path.GetFileName(filePath))
        {
        }
    }
}
