using Feign.Internal;
using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// apply parameter request transform
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public abstract class RequestParameterTransformBaseAttribute : Attribute, IRequestTransformParameter
    {
        protected internal abstract LocalBuilder? EmitNewHttpRequestTransform(ILGenerator iLGenerator, LocalBuilder valueBuilder);

    }
}
