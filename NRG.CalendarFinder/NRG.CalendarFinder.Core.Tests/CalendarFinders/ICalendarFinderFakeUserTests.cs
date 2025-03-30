using NRG.CalendarFinder.Core.CalendarFinders;
using NRG.CalendarFinder.Core.CalendarFinders.Models;
using NRG.CalendarFinder.Core.Tests.CalendarFinders.DI;

namespace NRG.CalendarFinder.Core.Tests.CalendarFinders;

[Category("Unit")]
[Category("CalendarFinder")]
[CalendarFinderDIFakeUser]
public class ICalendarFinderFakeUserTests(ICalendarFinder calfi)
{
    [Test]
    [Arguments("with-id")]
    [Arguments("with-upn")]
    [Arguments("with-both")]
    public async Task ValidUserEx(string userIdentifier)
    {
        var foundResult = await calfi.FindCalendarAsync(userIdentifier);

        var fex = await Assert.That(foundResult.Value).IsTypeOf<FoundException>().And.IsNotNull();
        var ex = await Assert.That(fex.Error).IsTypeOf<Exception>().And.IsNotNull();
        await Assert.That(fex.Error).HasMessageContaining("Problem finding calendars");
    }

    [Test]
    [Arguments("none")]
    [Arguments("")]
    public async Task InvalidUserEx(string userIdentifier)
    {
        var foundResult = await calfi.FindCalendarAsync(userIdentifier);

        var fex = await Assert.That(foundResult.Value).IsTypeOf<FoundException>().And.IsNotNull();
        var ex = await Assert.That(fex.Error).IsTypeOf<ArgumentException>().And.IsNotNull();
        await Assert.That(ex).HasMessageContaining("is null");
    }
}
