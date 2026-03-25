using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET45
using ServiceIdKey = System.Tuple<string, string>;
using ServiceTypeKey = System.Tuple<string, System.Type>;
#else
using ServiceIdKey = System.Tuple<string, string>;
using ServiceTypeKey = System.ValueTuple<string, System.Type>;
#endif

namespace Feign.Pipeline.Internal
{
    partial class GlobalFeignClientPipeline
    {
        private readonly IDictionary<ServiceIdKey, KeyedServiceIdFeignClientPipeline> _keyedServiceIdPipelineMap = new Dictionary<ServiceIdKey, KeyedServiceIdFeignClientPipeline>();
        private readonly IDictionary<ServiceTypeKey, IServiceFeignClientPipeline<object>> _keyedServiceTypePipelineMap = new Dictionary<ServiceTypeKey, IServiceFeignClientPipeline<object>>();
        public KeyedServiceIdFeignClientPipeline? GetKeyedServicePipeline(string? key, string serviceId)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            _keyedServiceIdPipelineMap.TryGetValue(new ServiceIdKey(key, serviceId), out var pipeline);
            return pipeline;
        }
        public KeyedServiceIdFeignClientPipeline KeyedService(string key, string serviceId)
        {
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                throw new ArgumentException(nameof(serviceId));
            }
            var keyValue = new ServiceIdKey(key, serviceId);
            if (_keyedServiceIdPipelineMap.TryGetValue(keyValue, out var pipeline))
            {
                return pipeline;
            }
            pipeline = new KeyedServiceIdFeignClientPipeline(key, serviceId);
            _keyedServiceIdPipelineMap[keyValue] = pipeline;
            return pipeline;
        }

        public KeyedServiceFeignClientPipeline<TService>? GetKeyedServicePipeline<TService>(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            _keyedServiceTypePipelineMap.TryGetValue(new ServiceTypeKey(key, typeof(TService)), out var pipeline);
            return pipeline as KeyedServiceFeignClientPipeline<TService>;
        }
        public KeyedServiceFeignClientPipeline<TService> KeyedService<TService>(string key)
        {
            var keyValue = new ServiceTypeKey(key, typeof(TService));
            if (_keyedServiceTypePipelineMap.TryGetValue(keyValue, out var pipeline))
            {
                return (KeyedServiceFeignClientPipeline<TService>)pipeline;
            }
            var temp = new KeyedServiceFeignClientPipeline<TService>(key);
            pipeline = (IServiceFeignClientPipeline<object>)temp;
            _keyedServiceTypePipelineMap[keyValue] = pipeline;
            return (KeyedServiceFeignClientPipeline<TService>)pipeline;
        }

        IFeignClientPipeline<object> IGlobalFeignClientPipeline.KeyedService(string key, string serviceId)
        {
            return KeyedService(key, serviceId);
        }
        IFeignClientPipeline<TService> IGlobalFeignClientPipeline.KeyedService<TService>(string key)
        {
            return KeyedService<TService>(key);
        }

    }
}
