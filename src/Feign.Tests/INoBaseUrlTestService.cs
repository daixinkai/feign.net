using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    [CustomFeignClient(""
        //, Fallback = typeof(TestControllerServiceFallback)
        , Url = ""
    )]
    public interface INoBaseUrlTestService
    {
        [GetMapping("https://www.baidu.com")]
        Task<string> GetBaidu();
        [GetMapping("https://www.baidu.com")]
        Task<HttpResponseMessage> GetBaiduResponse();
    }
}
