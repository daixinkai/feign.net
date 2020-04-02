using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    public interface IQueryResult
    {
        /// <summary>
        /// 状态码
        /// </summary>
        HttpStatusCode StatusCode { get; }
        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        bool IsSuccessStatusCode { get; }
    }

    public interface IQueryResult<T> : IQueryResult
    {
        T Data { get; }
    }

}
