using Microsoft.Extensions.DependencyInjection;
using NRG.CalendarFinder.Core.Certificates;

namespace NRG.CalendarFinder.Core.Tests.Certificates.DI;

public class WindowsCertificateDIAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly ServiceProvider _serviceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
        => _serviceProvider.CreateAsyncScope();

    public override object? Create(IServiceScope scope, Type type)
        => scope.ServiceProvider.GetService(type);

    private static ServiceProvider CreateSharedServiceProvider()
        => new ServiceCollection()
            .AddSingleton<ICertificateCreate, WindowsCertificateHandler>()
            .AddSingleton<ICertificateGet, WindowsCertificateHandler>()
            .AddSingleton<ICertificateAdd, WindowsCertificateHandler>()
            .AddSingleton<ICertificateRemove, WindowsCertificateHandler>()
            .BuildServiceProvider();
}