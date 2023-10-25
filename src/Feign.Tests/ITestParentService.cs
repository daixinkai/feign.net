using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    [CustomFeignClient("yun-platform-service-provider"
    , Fallback = typeof(TestServiceFallback)
    //, FallbackFactory = typeof(TestServiceFallbackFactory)
    //, Url = "http://localhost:8802/"
    //, Url = "http://10.1.5.90:8802/"
    //, Url = "http://localhost:62088/"
    //, Url = "http://www.baidu.com/"
    , Url = "https://www.jd.com/"
    //, Configuration = typeof(TestConfiguration)
    )]
    [NonFeignClient]
    public interface ITestParentService<TModel>
    {
        Type ServiceType { get; set; }
        string ServiceId { get; set; }
        string Name { get; set; }
        [RequestMapping("/{id}", Method = "GET")]
        [MethodId("GetById")]
        Task<TModel> GetById(object id);
    }
}
