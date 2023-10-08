using Feign;
using Feign.Tests;
using Feign.TestWeb;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

var feignBuilder = builder.Services.AddFeignClients(options =>
{
    //options.DiscoverServiceCacheTime = TimeSpan.FromSeconds(10);
}).ConfigureJsonSettings(options =>
{
    options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
})
 .AddTestFeignClients()
//.AddServiceDiscovery<TestServiceDiscovery>()
//.AddSteeltoe()
;
feignBuilder.Options.FeignClientPipeline.Service<IAngleSharpTestService>().AddAngleSharp();
//feignBuilder.AddPolly(options =>
//{
//    options.Configure(asyncPolicy =>
//    {
//        return Policy.WrapAsync(
//           asyncPolicy,
//           Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
//        );
//    });
//    options.ConfigureAsync(async asyncPolicy =>
//    {
//        await Task.FromResult(0);
//        return Policy.WrapAsync(
//                  asyncPolicy,
//                  Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
//             );
//    });
//    options.Configure("serviceId", asyncPolicy =>
//    {
//        return Policy.WrapAsync(
//           asyncPolicy,
//           Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
//        );
//    });
//    options.Configure<ITestService>(asyncPolicy =>
//    {
//        return Policy.WrapAsync(
//           asyncPolicy,
//           Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))
//        );
//    });
//});
feignBuilder.Options.FeignClientPipeline.UseInitializing(context =>
{
    context.HttpClient.DefaultRequestVersion = new Version(2, 0);
});

feignBuilder.Options.FeignClientPipeline.UseReceivedResponse(context =>
{
    var result = context.Result;
    return default;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
