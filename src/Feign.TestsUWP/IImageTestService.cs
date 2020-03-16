using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsUWP
{
    [FeignClient("image-test-service", Url = "https://pics5.baidu.com")]
    public interface IImageTestService
    {
        [GetMapping("/{uri}", ContentType = "text/html")]
        Task<Stream> GetImage(string uri);
    }
}
