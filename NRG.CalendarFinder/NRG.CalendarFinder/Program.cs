using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NRG.CalendarFinder.Exteinsions;
using NRG.CalendarFinder.Models;

namespace NRG.CalendarFinder;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(async options =>
            {
                await Console.Out.WriteLineAsync($"Start App.");

                try
                {
                    var host = Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration(builder =>
                        {
                            builder.AddJsonFile(options.AppSettingsPath);
                        })
                        .ConfigureServices((context, services) =>
                        {
                            // Services 
                            services.AddSingleton(options);
                            services.AddSingleton<CalendarFinderService>();

                            // Workers
                            services.AddHostedService<CalendarFinderWorker>();
                        })
                        .AddGraphClientsFromJson()
                        .UseConsoleLifetime()
                        .ConfigureLogging(e => e.SetMinimumLevel(LogLevel.None))
                        .Build();

                    await host.RunAsync();
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Failed with error: {ex.Message}");
                }
                finally
                {
                    await Console.Out.WriteLineAsync($"Terminate App.");
                }
            });
    }
}
