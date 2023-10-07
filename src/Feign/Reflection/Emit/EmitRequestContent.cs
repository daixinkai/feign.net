using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    internal class EmitRequestContent
    {
        public EmitRequestContent()
        {
            ParameterIndex = -1;
        }

        public string MediaType { get; set; } = null!;
        /// <summary>
        /// 参数描述
        /// </summary>
        public ParameterInfo Parameter { get; set; } = null!;
        /// <summary>
        /// 此参数所在的索引
        /// </summary>
        public int ParameterIndex { get; set; }
        /// <summary>
        /// 是否支持多参数
        /// </summary>
        public bool SupportMultipart { get; set; }

    }
}
