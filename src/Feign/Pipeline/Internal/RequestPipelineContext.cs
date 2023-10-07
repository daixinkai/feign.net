using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
//#if NET5_0_OR_GREATER
//    internal partial record FullRequestPipelineContext<TService> : FeignClientPipelineContext<TService>,
//#else
    internal partial class RequestPipelineContext<TService> : FeignClientPipelineContext<TService>,
//#endif
    IBuildingRequestPipelineContext<TService>,
    ISendingRequestPipelineContext<TService>,
    ICancelRequestPipelineContext<TService>
    {
        public RequestPipelineContext(
            IFeignClient<TService> feignClient,
            FeignHttpRequestMessage requestMessage,
            CancellationToken cancellationToken
            ) : base(feignClient)
        {
            _requestMessage = requestMessage;
            RequestUri = _requestMessage.RequestUri;
            Headers = new Dictionary<string, string>();
            CancellationToken = cancellationToken;
        }

        private FeignHttpRequestMessage _requestMessage;

        public FeignClientHttpRequest Request => _requestMessage.FeignClientRequest;

        public FeignHttpRequestMessage RequestMessage => _requestMessage;


        #region BuildingRequest
        public string Method => _requestMessage.Method.ToString();
        public Uri? RequestUri { get; set; }
        public IDictionary<string, string> Headers { get; }
        #endregion

        #region SendingRequest

        private bool _isTerminated;
        internal CancellationTokenSource? _cancellationTokenSource;
        /// <summary>
        /// 获取一个值,指示是否终止请求
        /// </summary>
        public bool IsTerminated => _isTerminated;
        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
                }
                return _cancellationTokenSource;
            }
        }
        public void Terminate()
        {
            _isTerminated = true;
        }

        #endregion

        #region CannelRequest
        public CancellationToken CancellationToken { get; }
        #endregion


    }
}
