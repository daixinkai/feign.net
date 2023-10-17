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
    internal partial class ResponsePipelineContext<TService, TResult> : FeignClientPipelineContext<TService>,
        IReceivedResponsePipelineContext<TService>,
        IReceivingResponsePipelineContext<TService>
    {
        public ResponsePipelineContext(
            IFeignClient<TService> feignClient,
            FeignClientHttpRequest request,
            HttpResponseMessage responseMessage
            ) : base(feignClient)
        {
            Request = request;
            ResponseMessage = responseMessage;
        }

        public FeignClientHttpRequest Request { get; }


        #region ReceivingResponse and ReceivingResponse

        public HttpResponseMessage ResponseMessage { get; set; }

        public Type ResultType => typeof(TResult);

        internal bool _isSetResult;

        internal TResult? _result;
        public object? Result
        {
            get
            {
                return _result;
            }
            set
            {
                _isSetResult = true;
#pragma warning disable CS8600
                _result = (TResult)value;
#pragma warning restore CS8600
            }
        }

        internal TResult? GetResult()
        {
            return _result;
        }
        #endregion

        /// <summary>
        /// is skip release the response
        /// </summary>
        public bool SkipReleaseResponse { get; set; }

    }

}
