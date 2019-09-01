using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole.NET45
{
    [FeignClient("baidu-service"
        , Url = "http://www.baidu.com")]
    public interface IBaiduTestService
    {
        string Get();
    }

    //[FeignClient("test-service", Url = "http://testservice.xx.com")]
    //public interface ITestService
    //{
    //    /// <summary>
    //    /// get一个请求
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    [RequestMapping("/{id}", Method = "GET");
    //    //[GetMapping("/{id}")]
    //    string Get([PathVariable("id")]int id);

    //    /// <summary>
    //    /// 以json的方式post一个请求
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <param name="param"></param>
    //    /// <returns></returns>
    //    [RequestMapping("/{id}", Method = "POST")]
    //    //[PostMapping("/{id}")]
    //    string PostJson([PathVariable]int id, [RequestBody] TestServiceParam param);

    //    /// <summary>
    //    /// 以form表单的方式post一个请求
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <param name="param"></param>
    //    /// <returns></returns>
    //    [RequestMapping("/{id}", Method = "POST")]
    //    //[PostMapping("/{id}")]
    //    string PostForm(int id, [RequestForm] TestServiceParam param);

    //    /// <summary>
    //    /// 上传2个文件
    //    /// </summary>
    //    /// <param name="file1"></param>
    //    /// <param name="file2"></param>
    //    /// <returns></returns>
    //    [PostMapping("/upload")]
    //    string UploadFile(IHttpRequestFile file1, IHttpRequestFile file2);

    //    /// <summary>
    //    /// 上传多个文件
    //    /// </summary>
    //    /// <param name="file1"></param>
    //    /// <param name="file2"></param>
    //    /// <returns></returns>
    //    [PostMapping("/upload")]
    //    string UploadFile(IHttpRequestFileForm files);

    //}

}
