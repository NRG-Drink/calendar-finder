using Microsoft.Graph.Models;
using NRG.CalendarFinder.Core.CalendarFinders.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.Tests.MsGraph.Formatters.Data;

public static class MsGraphFormatterTestData
{
    #region Users
    public static User MrIncognito { get; } = new()
    {
        Id = $"{Guid.NewGuid()}",
        DisplayName = "Mr. Incognito",
        UserPrincipalName = "incognito@anonymous.nex",
        Mail = "incognito@anonymous.nex",
    };

    public static User User1 => new()
    {
        Id = $"{Guid.NewGuid()}",
        DisplayName = "User One",
        UserPrincipalName = "hello@world.com",
        Mail = "$äöü@umlauts.ch",
    };

    public static User User2 => new()
    {
        Id = $"{Guid.NewGuid()}",
        DisplayName = "User Two",
        UserPrincipalName = "user.two@provider.com",
        Mail = "hello@world.com",
    };
    #endregion

    #region Calendars
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
        Owner = new() { Name = User2.DisplayName }
    };
    #endregion

    #region Errors
    public static Exception ErrorNull => new ArgumentException("'null' is no valid input.");
    #endregion

    public static IEnumerable<Func<(string, OneOf<FoundResult, FoundException>)>> FoundData()
    {
        yield return () => ("FoundException001", new FoundException(MrIncognito.DisplayName!, new($"{MrIncognito.DisplayName} not found.")));
        yield return () => ("FoundException002", new FoundException(MrIncognito.DisplayName!, ErrorNull));

        yield return () => ("FoundResult001 multiple calendars", new FoundResult(User1.DisplayName!, User1, [Calendar1, Calendar2]));
        yield return () => ("FoundResult002 multiple default calendars", new FoundResult(User1.DisplayName!, User1, [Calendar1, Calendar1]));
        yield return () => ("FoundResult003 no default calendar", new FoundResult(User2.DisplayName!, User2, [Calendar2]));
    }
}
