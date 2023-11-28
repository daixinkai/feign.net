# Usage
1. Install the NuGet package

    `PM> Install-Package Feign.DependencyInjection`

2. Add the services

    ```c#
        var feignBuilder = builder.Services.AddFeignClients(options =>
        {
            //options.DiscoverServiceCacheTime = TimeSpan.FromSeconds(10);
        }).AddFeignClients(typeof(ITestService).Assembly, FeignClientLifetime.Singleton)
        .ConfigureJsonSettings(options =>
        {
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });
    ```