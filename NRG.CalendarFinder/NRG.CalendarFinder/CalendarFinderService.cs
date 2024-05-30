using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using NRG.CalendarFinder.MsGraphFactories;

namespace NRG.CalendarFinder;

public class CalendarFinderService(IMsGraphClientFactory graphFactory)
{
	private readonly GraphServiceClient _graph = graphFactory.GetClient("GraphClient");

	public async Task<User> FindUserOrThrowAsync(string userIdentifier)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(userIdentifier))
			{
				throw new ArgumentException($"User identifier is null or whitespace.");
			}

			return await TryFindUserOrThrowAsync(userIdentifier);
		}
		catch (Exception ex)
		{
			throw new Exception($"Problem finding user ({userIdentifier})", ex);
		}
	}

	public async Task<List<Calendar>> FindCalendarsOrThrowAsync(User user)
	{
		try
		{
			var isUserValid = user is not null && (user.Id is not null || user.UserPrincipalName is not null);
			if (!isUserValid)
			{
				throw new ArgumentException($"User or User.Id or User.PrincipalName is null.");
			}

			return await TryFindCalendarsOrThrowAsync(user!);
		}
		catch (Exception ex)
		{
			throw new Exception(
				$"Problem finding calendars for user ({user.UserPrincipalName})", ex);
		}
	}

	private async Task<User> TryFindUserOrThrowAsync(string userIdentifier)
	{
		var user = await GetUserByIdentifier(userIdentifier);
		user ??= await UserStartsWith("mail", userIdentifier);

		if (user is null)
		{
			var nickname = GetMailNickname(userIdentifier);
			user ??= await UserStartsWith("mail", nickname);
			user ??= await UserStartsWith("userPrincipalName", nickname);
		}

		return user ?? throw new Exception(
			"No user could be found. Please check your spelling.");
	}

	private static string GetMailNickname(string userIdentifier)
		=> userIdentifier
			.Split("@")
			.FirstOrDefault()
			?? userIdentifier;

	private async Task<User?> GetUserByIdentifier(string userIdentifier)
	{
		try
		{
			return await _graph.Users[userIdentifier].GetAsync();
		}
		catch (ODataError)
		{
			return null;
		}
	}

	private async Task<User?> UserStartsWith(string property, string value)
	{
		var users = await _graph.Users.GetAsync(e =>
		{
			e.QueryParameters.Filter = $"startsWith({property},'{value}')";
		});

		return users?.Value?.FirstOrDefault();
	}

	private async Task<List<Calendar>> TryFindCalendarsOrThrowAsync(User user)
	{
		var response = await _graph
			.Users[user.Id ?? user.UserPrincipalName]
			.Calendars
			.GetAsync();

		return response?.Value
			?? throw new Exception("No calendars could be found.");
	}
}
