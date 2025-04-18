using NRG.CalendarFinder.Core.CalendarFinders.Models;
using NRG.CalendarFinder.Core.MsGraph;
using NRG.CalendarFinder.Core.Tests.MsGraph.Formatters.Data;
using NRG.CalendarFinder.Core.Tests.MsGraph.Formatters.DI;
using OneOf;

namespace NRG.CalendarFinder.Core.Tests.MsGraph.Formatters;
[Category("Unit")]
[Category("IMsGraphFormatter")]
[MsGraphFormatterDI]
public class IMsGraphFormatterTests(IMsGraphFormatter formatter)
{
    [Test]
    [MethodDataSource(typeof(MsGraphFormatterTestData), nameof(MsGraphFormatterTestData.FoundData))]
    [DisplayName("$name")]
    public async Task User(string name, OneOf<FoundResult, FoundException> result)
    {
        var formatted = formatter.Format(result);
        await Verify(formatted)
            .UseStrictJson()
            .UseParameters(name);
    }
}
