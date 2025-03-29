using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models;
using NRG.CalendarFinder.Core.CalendarFinders;
using NRG.CalendarFinder.Core.Extensions;
using NRG.CalendarFinder.Core.Models;
using NRG.CalendarFinder.Core.MsGraph;
using OneOf;

namespace NRG.CalendarFinder.Core.Tests.CalendarFinders.DI;

public class CalendarFinderDIFakeUserAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
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
            .ConfigureServices((context, services) =>
            {
                var appSettings = context.Configuration.Get<AppSettings>();
                ArgumentNullException.ThrowIfNull(appSettings);

                var graph = new MsGraphCreator().Create(appSettings.MsGraphCredential);

                services
                    .AddSingleton(appSettings)
                    .AddSingleton<IUserService, UserServiceFake>()
                    .AddSingleton<ICalendarService>(new CalendarService(graph))
                    .AddSingleton<ICalendarFinder, CalFi>();
            })
            .Build();

        return host.Services;
    }

    public class UserServiceFake : IUserService
    {
        public Task<OneOf<User, Exception>> FindUserAsync(string userIdentifier)
            => Task.FromResult<OneOf<User, Exception>>(GetFakeUser(userIdentifier));

        private static User GetFakeUser(string e)
            => e switch
            {
                "with-id" => new User() { Id = $"{Guid.NewGuid()}" },
                "with-upn" => new User() { UserPrincipalName = "hello@world.com" },
                "with-both" => new User() { Id = $"{Guid.NewGuid()}", UserPrincipalName = "hello@world.com" },
                _ => new User()
            };
    }
}
