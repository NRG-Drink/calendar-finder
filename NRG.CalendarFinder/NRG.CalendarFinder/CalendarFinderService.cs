using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
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

        var user = await TryFindUser(userIdentifier);

        if (user is null || user.Id is null)
        {
            throw new ArgumentException($"Found user is not valid for identifier: {userIdentifier}");
        }

        return user;
    }

    private async Task<User?> TryFindUser(string userIdentifier)
    {
        try
        {
            return await GetUserByIdentifier(userIdentifier);
        }
        catch (ODataError _)
        {
            var mailUser = await GetUserByMail(userIdentifier);
            if (mailUser is not null)
            {
                return mailUser;
            }

            var nickname = GetMailNickname(userIdentifier);

            var mailNicknameUser = await GetUserByMail(nickname);
            if (mailNicknameUser is not null)
            {
                return mailNicknameUser;
            }

            var principleUser = await GetUserByPrincipleName(nickname);

            return principleUser;
        }
    }

    private static string GetMailNickname(string userIdentifier)
        => userIdentifier
            .Split("@")
            .FirstOrDefault() 
            ?? userIdentifier;

    private async Task<User?> GetUserByPrincipleName(string nickname)
    {
        var users = await _graph.Users.GetAsync(e =>
        {
            e.QueryParameters.Filter = $"startsWith(userPrincipleName,'{nickname}')";
        });

        return users?.Value?.FirstOrDefault();
    }

    private async Task<User?> GetUserByMail(string nickname)
    {
        var users = await _graph.Users.GetAsync(e =>
        {
            e.QueryParameters.Filter = $"startsWith(mail,'{nickname}')";
        });

        return users?.Value?.FirstOrDefault();
    }
        

    private Task<User?> GetUserByIdentifier(string userIdentifier)
        => _graph.Users[userIdentifier].GetAsync();

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
