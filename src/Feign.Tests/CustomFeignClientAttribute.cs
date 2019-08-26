using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Feign.Tests
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
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

        string _url;

        bool Flag { get; }

        public override string Url { get => _url; set => _url = value; }

    }
}
