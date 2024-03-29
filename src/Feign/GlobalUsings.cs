﻿#if NETCOREAPP2_1_OR_GREATER
global using HttpHandlerType = System.Net.Http.SocketsHttpHandler;
#else
global using HttpHandlerType = System.Net.Http.HttpClientHandler;
#endif

#if NET8_0_OR_GREATER
global using System.Collections.Frozen;
#endif

#if USE_SYSTEM_TEXT_JSON
global using System.Text.Json;
global using JsonProviderType = Feign.Formatting.SystemTextJsonProvider;
#else
global using Newtonsoft.Json;
global using JsonSerializerOptions = Newtonsoft.Json.JsonSerializerSettings;
global using JsonProviderType = Feign.Formatting.NewtonsoftJsonProvider;
#endif

#if !USE_VALUE_TASK
global using ValueTask = System.Threading.Tasks.Task;
#endif