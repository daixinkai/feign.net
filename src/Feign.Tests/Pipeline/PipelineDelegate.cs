using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    public delegate Task PipelineDelegate<T>(IPipelineContext<T> context);
}
