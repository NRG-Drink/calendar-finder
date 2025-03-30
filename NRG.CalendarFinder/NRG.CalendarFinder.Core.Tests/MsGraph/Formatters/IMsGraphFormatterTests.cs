using Microsoft.Graph.Models;
using NRG.CalendarFinder.Core.CalendarFinders.Models;
using NRG.CalendarFinder.Core.MsGraph;
using NRG.CalendarFinder.Core.Tests.MsGraph.Formatters.DI;
using OneOf;

namespace NRG.CalendarFinder.Core.Tests.MsGraph.Formatters;
[Category("Unit")]
[Category("IMsGraphFormatter")]
[MsGraphFormatterDI]
public class IMsGraphFormatterTests(IMsGraphFormatter formatter)
{
    #region Test Data
    public static User User1 => new()
    {
        Id = $"{Guid.NewGuid()}",
        DisplayName = "Hello World!",
        UserPrincipalName = "hello@world.com",
        Mail = "hello@wörld.com",
    };

    public static Calendar Calendar1 => new()
    {
        Id = $"{Guid.NewGuid()}",
        Name = "Calendar",
        IsDefaultCalendar = true,
        Owner = new() { Name = User1.DisplayName }
    };

    public static Calendar Calendar2 => new()
    {
        Id = $"{Guid.NewGuid()}",
        Name = "Holidays",
        IsDefaultCalendar = false,
        Owner = new() { Name = User1.DisplayName }
    };

    public static Exception Error1 => new("Hello from exception.");

    public static IEnumerable<Func<(string, OneOf<FoundResult, FoundException>)>> FoundData()
    {
        yield return () => ("Normal1", new FoundResult(User1.DisplayName!, User1, [Calendar1, Calendar2]));
        yield return () => ("Ex1",  new FoundException("Mr. Incognito", Error1));
    }
    #endregion

    [Test]
    [MethodDataSource(nameof(FoundData))]
    [DisplayName("$name")]
    public async Task User(string name, OneOf<FoundResult, FoundException> result)
    {
        var formatted = formatter.Format(result);
        await Verify(formatted)
            .UseStrictJson()
            .UseParameters(name);
    }
}
