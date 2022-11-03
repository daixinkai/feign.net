using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal class GlobalFeignClientPipeline : FeignClientPipelineBase<object>, IGlobalFeignClientPipeline
    {
        private readonly IDictionary<string, ServiceIdFeignClientPipeline> _serviceIdPipelineMap = new Dictionary<string, ServiceIdFeignClientPipeline>();
        private readonly IDictionary<Type, IServiceFeignClientPipeline<object>> _serviceTypePipelineMap = new Dictionary<Type, IServiceFeignClientPipeline<object>>();
        public ServiceIdFeignClientPipeline GetServicePipeline(string serviceId)
        {
            _serviceIdPipelineMap.TryGetValue(serviceId, out var serviceFeignClientPipeline);
            return serviceFeignClientPipeline;
        }
        public ServiceIdFeignClientPipeline GetOrAddServicePipeline(string serviceId)
        {
            if (_serviceIdPipelineMap.TryGetValue(serviceId, out var serviceFeignClientPipeline))
            {
                return serviceFeignClientPipeline;
            }
            serviceFeignClientPipeline = new ServiceIdFeignClientPipeline(serviceId);
            _serviceIdPipelineMap[serviceId] = serviceFeignClientPipeline;
            return serviceFeignClientPipeline;
        }

        public ServiceFeignClientPipeline<TService> GetServicePipeline<TService>()
        {
            _serviceTypePipelineMap.TryGetValue(typeof(TService), out var serviceFeignClientPipeline);
            return serviceFeignClientPipeline as ServiceFeignClientPipeline<TService>;
        }
        public ServiceFeignClientPipeline<TService> GetOrAddServicePipeline<TService>()
        {
            if (_serviceTypePipelineMap.TryGetValue(typeof(TService), out var serviceFeignClientPipeline))
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
