using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Feign;
using Feign.Tests;
using Polly;

namespace Feign.TestWeb.NETCORE30
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddRazorPages();

            var builder = services.AddFeignClients(options =>
            {
                //options.DiscoverServiceCacheTime = TimeSpan.FromSeconds(10);
            }).ConfigureJsonSettings(options =>
            {
                options.IgnoreNullValues = true;
                options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            })
              .AddTestFeignClients()
            //.AddServiceDiscovery<TestServiceDiscovery>()
            //.AddSteeltoe()
            ;
            builder.Options.FeignClientPipeline.Service<IAngleSharpTestService>().AddAngleSharp();
            builder.AddPolly(options =>
            {
                options.Configure(asyncPolicy =>
                {
                    return Policy.WrapAsync(
                       asyncPolicy,
                       Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
                    );
                });
                options.ConfigureAsync(async asyncPolicy =>
                {
                    await Task.FromResult(0);
                    return Policy.WrapAsync(
                              asyncPolicy,
                              Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
                         );
                });
                options.Configure("serviceId", asyncPolicy =>
                {
                    return Policy.WrapAsync(
                       asyncPolicy,
                       Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
                    );
                });
                options.Configure<ITestService>(asyncPolicy =>
                {
                    return Policy.WrapAsync(
                       asyncPolicy,
                       Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
                    );
                });
            });
            builder.Options.FeignClientPipeline.Initializing += (sender, e) =>
            {
                e.HttpClient.DefaultRequestVersion = new Version(2, 0);
            };

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
