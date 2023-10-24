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
        public ServiceIdFeignClientPipeline? GetServicePipeline(string serviceId)
        {
            _serviceIdPipelineMap.TryGetValue(serviceId, out var serviceFeignClientPipeline);
            return serviceFeignClientPipeline;
        }
        public ServiceIdFeignClientPipeline Service(string serviceId)
        {
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                throw new ArgumentException(nameof(serviceId));
            }
            if (_serviceIdPipelineMap.TryGetValue(serviceId, out var serviceFeignClientPipeline))
            {
                return serviceFeignClientPipeline;
            }
            serviceFeignClientPipeline = new ServiceIdFeignClientPipeline(serviceId);
            _serviceIdPipelineMap[serviceId] = serviceFeignClientPipeline;
            return serviceFeignClientPipeline;
        }

        public ServiceFeignClientPipeline<TService>? GetServicePipeline<TService>()
        {
            _serviceTypePipelineMap.TryGetValue(typeof(TService), out var serviceFeignClientPipeline);
            return serviceFeignClientPipeline as ServiceFeignClientPipeline<TService>;
        }
        public ServiceFeignClientPipeline<TService> Service<TService>()
        {
            if (_serviceTypePipelineMap.TryGetValue(typeof(TService), out var serviceFeignClientPipeline))
            {
                return (ServiceFeignClientPipeline<TService>)serviceFeignClientPipeline;
            }
            var temp = new ServiceFeignClientPipeline<TService>();
            serviceFeignClientPipeline = (IServiceFeignClientPipeline<object>)temp;
            _serviceTypePipelineMap[typeof(TService)] = serviceFeignClientPipeline;
            return (ServiceFeignClientPipeline<TService>)serviceFeignClientPipeline;
        }
        IFeignClientPipeline<object> IGlobalFeignClientPipeline.Service(string serviceId)
        {
            return Service(serviceId);
        }
        IFeignClientPipeline<TService> IGlobalFeignClientPipeline.Service<TService>()
        {
            return Service<TService>();
        }
    }
}
