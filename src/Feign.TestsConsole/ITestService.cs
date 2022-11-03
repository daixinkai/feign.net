using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole
{
    [FeignClient("test-service"
        , Url = "http://localhost:62488")]
    public interface ITestService
    {
        [GetMapping("html")]
        Task<string> GetHtmlAsync();
    }

}
