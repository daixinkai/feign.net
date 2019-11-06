using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feign.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Feign.TestWeb.NETCORE30.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet([FromServices]ITestService testService)
        {
            //var r = testService.GetById(1).Result;
            var r = testService.GetQueryResultValueAsync("", new TestServiceParam { }).Result;
            testService.PostValueAsync().Wait();
        }
    }
}
