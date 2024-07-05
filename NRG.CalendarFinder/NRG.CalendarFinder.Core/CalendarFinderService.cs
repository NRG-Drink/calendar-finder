using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using NRG.CalendarFinder.Core.Models;
using NRG.CalendarFinder.Core.MsGraphFactories;

namespace NRG.CalendarFinder.Core;

public class CalendarFinderService(GraphServiceClient graph)
{
	public async Task<OutputData> ProcessInputAsync(InputData input)
	{
		var output = new OutputData() { Input = input };
		try
		{
			var user = await FindUserOrThrowAsync(input.UserIdentifier);
			output = output.WithUser(user);
			var calendars = await FindCalendarsOrThrowAsync(user);
			output = output.WithCalendars(calendars);
		}
		catch (ODataError oex)
		{
			output = output.WithError(oex);
		}
		catch (Exception ex)
		{
			output = output.WithError(ex);
		}
		finally
		{
			await Console.Out.WriteLineAsync(
				$"found: {output.Error is null,-5} - {input.UserIdentifier}");
		}

		return output;
	}

    private async Task<User> FindUserOrThrowAsync(string userIdentifier)
	{
		try
		{
			return string.IsNullOrWhiteSpace(userIdentifier)
				? throw new ArgumentException($"User identifier is null or whitespace.")
				: await TryFindUserOrThrowAsync(userIdentifier);
		}
		catch (Exception ex)
		{
			throw new Exception($"Problem finding user ({userIdentifier})", ex);
		}
	}

    private async Task<List<Calendar>> FindCalendarsOrThrowAsync(User user)
	{
		try
		{
			var isUserValid = user is not null && (user.Id is not null || user.UserPrincipalName is not null);
			return !isUserValid
				? throw new ArgumentException($"User or User.Id or User.PrincipalName is null.")
				: await TryFindCalendarsOrThrowAsync(user!);
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