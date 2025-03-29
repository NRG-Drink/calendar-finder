using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRG.CalendarFinder.Core.Extensions;

namespace NRG.CalendarFinder.Core.Tests.CalendarFinders.DI;

public class CalendarFinderDIAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider _serviceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
        => _serviceProvider.CreateAsyncScope();

    public override object? Create(IServiceScope scope, Type type)
        => scope.ServiceProvider.GetService(type);

    private static IServiceProvider CreateSharedServiceProvider()
    {
        var fileName = $"{nameof(CalendarFinderDIAttribute).TrimEnd([.. "Attribute"])}.appsettings.json";
        var filePath = Path.Combine("CalendarFinders", "DI", fileName);

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile(filePath);
            })
            .UseCalendarFinder()
            .Build();

        return host.Services;
    }
}