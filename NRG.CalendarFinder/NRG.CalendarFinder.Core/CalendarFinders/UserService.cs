using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public class UserService(GraphServiceClient graph) : IUserService
{
    public async Task<OneOf<User, Exception>> FindUserAsync(string userIdentifier)
    {
        if (string.IsNullOrWhiteSpace(userIdentifier) || userIdentifier.Length is 0)
        {
            return new ArgumentException($"User identifier is null, empty or whitespace and can therefore not be progressed.");
        }

        try
        {
            return await TryFindUserAsync(userIdentifier);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private async Task<OneOf<User, Exception>> TryFindUserAsync(string userIdentifier)
    {
        var user = await GetUserByIdentifier(userIdentifier);
        user ??= await UserStartsWith("mail", userIdentifier);

        if (user is null)
        {
            var nickname = GetMailNickname(userIdentifier);
            user ??= await UserStartsWith("mail", nickname);
            user ??= await UserStartsWith("userPrincipalName", nickname);
        }

        return user is null
            ? new Exception("No user could be found. Please check your spelling.")
            : user;
    }

    private async Task<User?> GetUserByIdentifier(string userIdentifier)
    {
        try
        {
            return await graph.Users[userIdentifier].GetAsync();
        }
        catch (ODataError)
        {
            return null;
        }
    }

    private async Task<User?> UserStartsWith(string property, string value)
    {
        var users = await graph.Users.GetAsync(e =>
        {
            e.QueryParameters.Filter = $"startsWith({property},'{value}')";
        });

        return users?.Value?.FirstOrDefault();
    }

    private static string GetMailNickname(string userIdentifier)
        => userIdentifier
            .Split("@")
            .FirstOrDefault()
            ?? userIdentifier;
}
