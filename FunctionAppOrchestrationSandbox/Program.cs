using FunctionAppOrchestrationSandbox;
using Microsoft.Extensions.Hosting;

var host = new FunctionHostBuilder().Build();
await host.RunAsync();