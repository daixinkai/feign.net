using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal class FilterFeignClientPipeline : FeignClientPipelineBase<object>, IFeignClientPipeline<object>
    {
        private readonly List<Func<IFeignClient, bool>> _filters;

    }
}
