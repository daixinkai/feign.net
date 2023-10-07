﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    /// <summary>
    /// 描述一个代理类型
    /// </summary>
    public class FeignClientTypeInfo
    {
        public FeignClientTypeInfo(FeignClientAttribute feignClient, Type serviceType)
        {
            FeignClient = feignClient;
            ServiceType = serviceType;
            Methods = new List<FeignClientMethodInfo>();
        }
        public FeignClientAttribute FeignClient { get; }
        /// <summary>
        /// 获取服务类型
        /// </summary>
        public Type ServiceType { get; }
        /// <summary>
        /// 获取或设置父类型
        /// </summary>
        public Type? ParentType { get; set; }
        /// <summary>
        /// 获取或设置生成的类型
        /// </summary>
        public Type? BuildType { get; set; }
        /// <summary>
        /// 获取方法集合
        /// </summary>
        public List<FeignClientMethodInfo> Methods { get; }

    }
}
