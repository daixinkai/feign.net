# feign.net for Steeltoe

Usage：

```csharp
services.AddDiscoveryClient(Configuration);

var builder = services.AddFeignClients();

builder.AddSteeltoe();

```