using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartMeterToMqtt.Providers;
using SmartMeterToMqtt.Infrastructure;

namespace SmartMeterToMqtt
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddTransient<ISmartMeterSerialProvider, SmartMeterSerialProvider>();
                    services.AddTransient<IMqttProvider, MqttProvider>();
                    services.AddHostedService<Worker>();
                })
                .ConfigureLogging((loggingContext, services) =>
                {
                    services.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}