using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRG.CalendarFinder.Core.CalendarFinders;
using NRG.CalendarFinder.Core.Models;
using NRG.CalendarFinder.Core.MsGraph;

namespace NRG.CalendarFinder.Core.Extensions;

public static class IHostBuilderExtensions
{
    public static IHostBuilder UseCalendarFinder(this IHostBuilder host)
    {
        host.ConfigureServices((context, services) =>
        {
            var appSettings = context.Configuration.Get<AppSettings>();
            ArgumentNullException.ThrowIfNull(appSettings);

            var graph = new MsGraphCreator().Create(appSettings.MsGraphCredential);

            services
                .AddSingleton(appSettings)
                .AddSingleton<IUserService>(new UserService(graph))
                .AddSingleton<ICalendarService>(new CalendarService(graph))
                .AddSingleton<ICalendarFinder, CalFi>();
        });

        return host;
    }
}
