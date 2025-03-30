using Microsoft.Graph;
using Microsoft.Graph.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public class CalendarService(GraphServiceClient graph) : ICalendarService
{
    public async Task<OneOf<List<Calendar>, Exception>> FindCalendarAsync(User user)
    {
        if (user is null || (user.Id is null && user.UserPrincipalName is null))
        {
            return new ArgumentException($"User or User.Id or User.PrincipalName is null.");
        }

        try
        {
            return await TryFindCalendarsOrThrowAsync(user!);
        }
        catch (Exception ex)
        {
            return new Exception($"Problem finding calendars for user ({user.UserPrincipalName})", ex);
        }
    }

    private async Task<List<Calendar>> TryFindCalendarsOrThrowAsync(User user)
    {
        var response = await graph
            .Users[user.Id ?? user.UserPrincipalName]
            .Calendars
            .GetAsync();

        return response?.Value
            ?? throw new Exception("No calendars could be found.");
    }
}
