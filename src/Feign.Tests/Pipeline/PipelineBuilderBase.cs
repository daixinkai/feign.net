using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    public abstract class PipelineBuilderBase<TService> : IPipelineBuilder<TService>
    {
        //public virtual PipelineDelegate<TService> New()
        //{
        //    return new PipelineDelegate<TService>(context => TaskEx.CompletedTask);
        //}
    }
}
