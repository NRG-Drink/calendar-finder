using NRG.CalendarFinder.Core.CalendarFinders.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public class CalFi(
    IUserService userService,
    ICalendarService calService
    ) : ICalendarFinder
{
    public async Task<OneOf<Found, Exception>> FindCalendarAsync(string userIdentifier)
    {
        var userResult = await userService.FindUserAsync(userIdentifier);
        if (userResult.TryPickT1(out var userEx, out var user))
        {
            return userEx;
        }

        var calendarsResult = await calService.FindCalendarAsync(user);
        if (calendarsResult.TryPickT1(out var calendarsEx, out var calendars))
        {
            return calendarsEx;
        }

        return new Found(userIdentifier, user, [.. calendars]);
    }
}
