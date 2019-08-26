using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    class GlobalFeignClientPipeline : FeignClientPipelineBase<object>, IGlobalFeignClientPipeline
    {
        IDictionary<string, ServiceIdFeignClientPipeline> _serviceIdPipelineMap = new Dictionary<string, ServiceIdFeignClientPipeline>();
        IDictionary<Type, IServiceFeignClientPipeline<object>> _serviceTypePipelineMap = new Dictionary<Type, IServiceFeignClientPipeline<object>>();
        public ServiceIdFeignClientPipeline GetServicePipeline(string serviceId)
        {
            ServiceIdFeignClientPipeline serviceFeignClientPipeline;
            _serviceIdPipelineMap.TryGetValue(serviceId, out serviceFeignClientPipeline);
            return serviceFeignClientPipeline;
        }
        public ServiceIdFeignClientPipeline GetOrAddServicePipeline(string serviceId)
        {
            ServiceIdFeignClientPipeline serviceFeignClientPipeline;
            if (_serviceIdPipelineMap.TryGetValue(serviceId, out serviceFeignClientPipeline))
            {
                return serviceFeignClientPipeline;
            }
            serviceFeignClientPipeline = new ServiceIdFeignClientPipeline(serviceId);
            _serviceIdPipelineMap[serviceId] = serviceFeignClientPipeline;
            return serviceFeignClientPipeline;
        }

        public ServiceFeignClientPipeline<TService> GetServicePipeline<TService>()
        {
            IServiceFeignClientPipeline<object> serviceFeignClientPipeline;
            _serviceTypePipelineMap.TryGetValue(typeof(TService), out serviceFeignClientPipeline);
            return serviceFeignClientPipeline as ServiceFeignClientPipeline<TService>;
        }
        public ServiceFeignClientPipeline<TService> GetOrAddServicePipeline<TService>()
        {
            IServiceFeignClientPipeline<object> serviceFeignClientPipeline;
            if (_serviceTypePipelineMap.TryGetValue(typeof(TService), out serviceFeignClientPipeline))
            {
                return serviceFeignClientPipeline as ServiceFeignClientPipeline<TService>;
            }

            var temp = new ServiceFeignClientPipeline<TService>();
            serviceFeignClientPipeline = temp as IServiceFeignClientPipeline<object>;
            _serviceTypePipelineMap[typeof(TService)] = serviceFeignClientPipeline;
            return serviceFeignClientPipeline as ServiceFeignClientPipeline<TService>;
        }

        IFeignClientPipeline<object> IGlobalFeignClientPipeline.GetServicePipeline(string serviceId)
        {
            return GetServicePipeline(serviceId);
        }

        IFeignClientPipeline<object> IGlobalFeignClientPipeline.GetOrAddServicePipeline(string serviceId)
        {
            return GetOrAddServicePipeline(serviceId);
        }
        IFeignClientPipeline<TService> IGlobalFeignClientPipeline.GetServicePipeline<TService>()
        {
            return GetServicePipeline<TService>();
        }

        IFeignClientPipeline<TService> IGlobalFeignClientPipeline.GetOrAddServicePipeline<TService>()
        {
            return GetOrAddServicePipeline<TService>();
        }
    }
}
