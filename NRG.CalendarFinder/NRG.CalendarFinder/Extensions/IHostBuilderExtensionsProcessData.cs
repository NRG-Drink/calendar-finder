using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRG.CalendarFinder.Models;

namespace NRG.CalendarFinder.Extensions;

public static class IHostBuilderExtensionsProcessData
{
    public static IHostBuilder AddProcessDataFromJson(this IHostBuilder builder, Options options)
    {
        builder.ConfigureServices((context, services) =>
        {
            var inputData = context.Configuration
                .GetSection("Data")
                .GetChildren()
                .Select(e => e.Value)
                .OfType<string>()
                .Select(e => new InputData() { UserIdentifier = e })
                .ToArray();

            var data = new ProcessData()
            {
                Options = options,
                Inputs = inputData
            };

            services.AddSingleton(data);
        });

        return builder;
    }
}
