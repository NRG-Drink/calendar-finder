using NRG.CalendarFinder.Core.CalendarFinders;
using NRG.CalendarFinder.Core.Tests.CalendarFinders.DI;

namespace NRG.CalendarFinder.Core.Tests.CalendarFinders;

[Category("Unit")]
[Category("CalendarFinder")]
[CalendarFinderDIFakeBoth]
public class ICalendarFinderFakeBothTests(ICalendarFinder calfi)
{
    [Test]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments("norris")]
    public async Task InvalidUserEx(string userIdentifier)
    {
        var foundResult = await calfi.FindCalendarAsync(userIdentifier);

        var ex = await Assert.That(foundResult.Value).IsTypeOf<Exception>().And.IsNotNull();
        await Assert.That(ex).HasMessageContaining(nameof(CalendarFinderDIFakeBothAttribute.CalendarServiceFake));
    }
}
