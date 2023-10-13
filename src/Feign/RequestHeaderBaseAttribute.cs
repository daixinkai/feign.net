using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Convert parameters into request headers
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public abstract class RequestHeaderBaseAttribute : Attribute, INotRequestParameter
    {
        protected internal abstract LocalBuilder EmitNewRequestHeaderHandler(ILGenerator iLGenerator, LocalBuilder valueBuilder);
    }
}
