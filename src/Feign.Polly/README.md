# feign.net Polly组件

Usage：

```csharp
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
```