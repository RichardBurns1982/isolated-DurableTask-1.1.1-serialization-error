using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunctionAppOrchestrationSandbox
{
    public class FunctionHostBuilder
    {
        public IHost Build()
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            return hostBuilder
                .ConfigureAppConfiguration(Startup.Configure)
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(Startup.Configure)
                .Build();
        }
    }
}
