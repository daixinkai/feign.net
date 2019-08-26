using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class RequestParamAttribute : Attribute, IRequestParameter
    {
        public RequestParamAttribute()
        {
        }
        public RequestParamAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
