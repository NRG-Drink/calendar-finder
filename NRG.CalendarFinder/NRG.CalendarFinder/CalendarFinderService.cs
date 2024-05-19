using Microsoft.Graph;
using Microsoft.Graph.Models;
using NRG.CalendarFinder.MsGraphFactories;

namespace NRG.CalendarFinder;

public class CalendarFinderService(IMsGraphClientFactory graphFactory)
{
    private readonly GraphServiceClient _graph = graphFactory.GetClient("GraphClient");

    public async Task<User> FindUserAsyncOrThrow(string userIdentifier)
    {
        if (string.IsNullOrWhiteSpace(userIdentifier))
        {
            throw new ArgumentException(userIdentifier, nameof(userIdentifier));
        }

        var user = await _graph.Users[userIdentifier].GetAsync();

        if (user is null || user.Id is null)
        {
            throw new ArgumentException($"Found user is not valid for identifier: {userIdentifier}");
        }

        return user;
    }

    public async Task<List<Calendar>> FindCalendarsAsyncOrThrow(User user)
    {
        var calendarResponse = await _graph.Users[user.Id ?? user.Mail].Calendars.GetAsync();

        if (calendarResponse?.Value is null)
        {
            throw new ArgumentException("Found calendars ar not valid.");
        }

        return calendarResponse.Value;
    }
}
