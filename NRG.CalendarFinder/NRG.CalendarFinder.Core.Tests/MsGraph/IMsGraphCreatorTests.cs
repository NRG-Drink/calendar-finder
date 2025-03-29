using Azure.Identity;
using FluentAssertions;
using NRG.CalendarFinder.Core.MsGraph;
using NRG.CalendarFinder.Core.Tests.MsGraph.Di;

namespace NRG.CalendarFinder.Core.Tests.MsGraph;
[Category("Local")]
[Category("IMsGraphCreator")]
[MsGraphCreatorDI]
public class IMsGraphCreatorTests(IMsGraphCreator creator)
{
    public static IEnumerable<Func<IMsGraphCredential>> GraphCredentials()
    {
        yield return () => new MsGraphCredential("123", "345", "B764B4104A1401BCEEA3986BDBCBF6F3290CC89D", []);
    }

    [Test]
    [MethodDataSource(nameof(GraphCredentials))]
    public async Task GetClientEx(IMsGraphCredential credential)
    {
        var graph = creator.Create(credential);
        var func = () => graph.Me.GetAsync();

        graph.Should().NotBeNull();
        await func.Should().ThrowExactlyAsync<AuthenticationFailedException>();
    }
}
