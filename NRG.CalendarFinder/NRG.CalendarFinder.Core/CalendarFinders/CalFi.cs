using NRG.CalendarFinder.Core.CalendarFinders.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public class CalFi(
    IUserService userService,
    ICalendarService calService
    ) : ICalendarFinder
{
    public async Task<OneOf<FoundResult, FoundException>> FindCalendarAsync(string userIdentifier)
    {
        var result = await RunAsync(userIdentifier);

        await Console.Out.WriteLineAsync(
                $"found: {result.Value is FoundResult,-5} - {userIdentifier}");

        return result;
    }

    private async Task<OneOf<FoundResult, FoundException>> RunAsync(string userIdentifier)
    {
        var userResult = await userService.FindUserAsync(userIdentifier);
        if (userResult.TryPickT1(out var userEx, out var user))
        {
            return new FoundException(userIdentifier, userEx);
        }

        var calendarsResult = await calService.FindCalendarAsync(user);
        if (calendarsResult.TryPickT1(out var calendarsEx, out var calendars))
        {
            return new FoundException(userIdentifier, calendarsEx);
        }

        return new FoundResult(userIdentifier, user, [.. calendars]);
    }
}
