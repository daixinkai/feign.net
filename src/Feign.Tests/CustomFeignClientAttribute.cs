using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Feign.Tests
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class CustomFeignClientAttribute : FeignClientAttribute
    {
        public CustomFeignClientAttribute(string name) : base(name)
        {
            MethodInfo method = null;
            if (Flag)
            {
                method = (MethodInfo)MethodBase.GetCurrentMethod();
            }
        }
        bool Flag { get; }
    }
}
