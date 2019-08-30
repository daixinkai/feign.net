using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole.NET45
{
    [FeignClient("baidu-service"
        , Url = "http://www.baidu.com")]
    public interface IBaiduTestService
    {
        string Get();
    }
}
