using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    class EmitRequestContent
    {
        public EmitRequestContent()
        {
            ParameterIndex = -1;
        }

        public string MediaType { get; set; }
        public ParameterInfo Parameter { get; set; }
        public int ParameterIndex { get; set; }

        public bool SupportMultipart { get; set; }

    }
}
