using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feign.Tests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Steeltoe.Discovery.Client;

namespace Feign.TestWeb
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //   services.AddDiscoveryClient(Configuration);

            var builder = services.AddFeignClients()
                .AddTestFeignClients()
                //.AddSteeltoeServiceDiscovery()
                ;
            builder.AddPolly(options =>
            {                
                options.Configure(asyncPolicy =>
                {
                    return Policy.WrapAsync(
                       asyncPolicy,
                       Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromSeconds(5))
                    );
                });
                options.Configure("serviceId",asyncPolicy =>
                {
                    return Policy.WrapAsync(
                       asyncPolicy,
                       Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromSeconds(5))
                    );
                });
                options.Configure<ITestService>(asyncPolicy =>
                {
                    return Policy.WrapAsync(
                       asyncPolicy,
                       Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromSeconds(5))
                    );
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();

            app.UseMvcWithDefaultRoute();
            //   app.UseDiscoveryClient();
        }
    }
}
