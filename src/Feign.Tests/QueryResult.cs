using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Feign.Tests
{
    /// <summary>
    /// 查询结果
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        public bool IsSuccessStatusCode
        {
            get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
        }
    }

    /// <summary>
    /// 查询结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResult<T> : QueryResult
    {
        public QueryResult()
        {
        }
        public QueryResult(T data)
        {
            Data = data;
        }
        /// <summary>
        /// 结果
        /// </summary>
        public T Data { get; set; }
                

    }
}
