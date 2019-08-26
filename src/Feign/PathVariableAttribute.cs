using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class PathVariableAttribute : Attribute, IRequestParameter
    {
        public PathVariableAttribute()
        {
        }
        public PathVariableAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
