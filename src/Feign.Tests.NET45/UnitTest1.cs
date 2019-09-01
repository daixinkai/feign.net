using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Castle.Windsor;
using Feign.Cache;
using Feign.Logging;
using Feign.Reflection;
using Feign.Request;
using Feign.Standalone;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Feign.Tests.NET45
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {            
            DynamicAssembly dynamicAssembly = new DynamicAssembly();
            dynamicAssembly.DEBUG_MODE = true;
            FeignClientHttpProxyTypeBuilder feignClientTypeBuilder = new FeignClientHttpProxyTypeBuilder(dynamicAssembly);
            FeignClientTypeInfo feignClientTypeInfo = feignClientTypeBuilder.Build(typeof(ITestService));
            feignClientTypeBuilder.Save();
        }

        [TestMethod]
        public void TestMethod_Autofac()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            IFeignBuilder feignBuilder = containerBuilder.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(ITestService).Assembly);
                options.Lifetime = FeignClientLifetime.Singleton;
                options.FeignClientPipeline.ReceivingQueryResult();
            });

            IContainer container = containerBuilder.Build();

            using (ILifetimeScope lifetimeScope = container.BeginLifetimeScope())
            {
                ITestService testService = lifetimeScope.Resolve<ITestService>();
                var result = testService.GetQueryResultValue("1", null);
            }



        }

        [TestMethod]
        public void TestMethod_CastleWindsor()
        {
            IWindsorContainer windsorContainer = new WindsorContainer();
            windsorContainer.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(ITestService).Assembly);
                options.FeignClientPipeline.ReceivingQueryResult();
            })
                .AddLoggerFactory<DefaultLoggerFactory>()
            ;
            ITestService testService = windsorContainer.Resolve<ITestService>();
            Assert.IsNotNull(testService);
            var result = testService.GetQueryResultValue("", null);
        }

        [TestMethod]
        public void TestMethod_Standalone()
        {
            bool b = IsPrime(10111);
            Assert.IsTrue(b);
            FeignClients.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(ITestService).Assembly);
                options.FeignClientPipeline.ReceivingQueryResult();
            });
            ITestService testService = FeignClients.Get<ITestService>();
            Assert.IsNotNull(testService);

            FilePathHttpRequestFile filePathRequestFile = new FilePathHttpRequestFile(@"E:\asdasdasd.txt");
            filePathRequestFile = null;

            //string value1 = testService.UploadFileAsync(filePathRequestFile, "asdasd").Result;
            //Assert.IsNotNull(value1);

            string value2 = testService.UploadFileAsync(new TestServiceUploadFileParam
            {
                Age = 1,
                Name = "asd111",
                File = new FilePathHttpRequestFile(@"E:\asdasdasd.txt")
                {
                    Name = "file"
                }
            }).Result;
            Assert.IsNotNull(value2);

            string value = testService.UploadFileAsync(filePathRequestFile, new TestServiceParam
            {
                Age = 1,
                Name = "asdasd"
            }).Result;
            Assert.IsNotNull(value);

            value = testService.FormTestAsync(new TestServiceParam
            {
                Age = 1,
                Name = "32424"
            }).Result;
            Assert.IsNotNull(value);

            value = testService.UploadFilesAsync(
                new FilePathHttpRequestFile(@"E:\asdasdasd.txt"),
                new FilePathHttpRequestFile(@"E:\asdasdasd.txt"),
                new FilePathHttpRequestFile(@"E:\asdasdasd.txt")
                ).Result;
            Assert.IsNotNull(value);

            Assert.AreNotEqual(value, "");
            var result = testService.GetQueryResultValue("", null);
        }

        [TestMethod]
        public void TestMethod_UploadFile()
        {
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            string path = @"E:\asdasdasd.txt";
            Stream fileStream = File.OpenRead(path);
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.Add("Content-Type", "application/octet-stream");
            //Content-Disposition: form-data; name="file"; filename="C:\B2BAssetRoot\files\596090\596090.1.mp4";
            streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + Path.GetFileName(path) + "\"");
            multipartFormDataContent.Add(streamContent, "file", Path.GetFileName(path));

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:62088/");
            httpClient.BaseAddress = new Uri("http://10.1.5.90:8802/");

            var response = httpClient.PostAsync("/api/values/uploadFile", multipartFormDataContent).Result;

            response.EnsureSuccessStatusCode();

            string value = response.Content.ReadAsStringAsync().Result;

        }


        [TestMethod]
        public void TestCustomOrderBy()
        {
            List<QueryResult> queryResults = new List<QueryResult>();

            queryResults.Add(new QueryResult()
            {
                StatusCode = System.Net.HttpStatusCode.Accepted
            });
            queryResults.Add(new QueryResult()
            {
                StatusCode = System.Net.HttpStatusCode.Ambiguous
            });
            queryResults.Add(new QueryResult()
            {
                StatusCode = System.Net.HttpStatusCode.BadGateway
            });

            queryResults.Add(new QueryResult()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
            queryResults.Add(new QueryResult()
            {
                StatusCode = System.Net.HttpStatusCode.Conflict
            });
            queryResults.Add(new QueryResult()
            {
                StatusCode = System.Net.HttpStatusCode.Continue
            });
            var query = queryResults.AsQueryable();

            var rr = query.OrderBy(s => s.StatusCode).OrderBy("StatusCode", true, true).ToList();

        }


        public static bool IsPrime(int number)
        {
            int times = 0;
            for (int i = 2; Math.Pow(i, 2) < number; ++i)
            {
                times++;
                if (number % i == 0)
                {
                    return false;
                }
            }
            Console.WriteLine(times);
            return true;
        }




    }


    public static class TestExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IOrderedQueryable<TEntity> source, string orderByProperty, bool desc, bool then)
        {
            var command = (then ? "Then" : "Order") + (desc ? "ByDescending" : "By");

            var entityType = typeof(TEntity);
            var entityParameter = Expression.Parameter(entityType, "x");

            var property = entityType.GetProperty(orderByProperty);

            var propertyAccess = Expression.MakeMemberAccess(entityParameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, entityParameter);

            var resultExpression =
                Expression.Call(typeof(Queryable), command, new Type[] { entityType, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));

            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }


}
