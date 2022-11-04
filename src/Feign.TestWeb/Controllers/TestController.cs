﻿using Feign.Tests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.TestWeb.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("{id}")]
        [HttpGet("")]
        public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync(string? id, [FromQuery] TestServiceParam param)
        {
            param.Name = param.Name + "_" + id + "_" + Guid.NewGuid().ToString();
            return Task.FromResult<IQueryResult<TestServiceParam>>(new QueryResult<TestServiceParam>()
            {
                Data = param,
                StatusCode = System.Net.HttpStatusCode.OK
            });
        }
    }
}
