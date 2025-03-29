using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public interface ICalendarFinder
{
    Task<OneOf<Found, Exception>> FindCalendarAsync(string userIdentifier);
}
