using Microsoft.Extensions.DependencyInjection;
using NRG.CalendarFinder.Core.MsGraph;

namespace NRG.CalendarFinder.Core.Tests.MsGraph.Di;

public class MsGraphCreatorDIAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly ServiceProvider _serviceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
        => _serviceProvider.CreateAsyncScope();

    public override object? Create(IServiceScope scope, Type type)
        => scope.ServiceProvider.GetService(type);

    private static ServiceProvider CreateSharedServiceProvider()
        => new ServiceCollection()
            .AddSingleton<IMsGraphCreator, MsGraphCreator>()
            .BuildServiceProvider();
}