using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunctionAppOrchestrationSandbox
{
    public static class Startup
    {
        public static void Configure(HostBuilderContext hostContext, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddEnvironmentVariables();
        }

        public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
        {
            var config = hostContext.Configuration;

        }
    }
}
