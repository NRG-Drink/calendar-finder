using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Text.Json.Serialization;

namespace NRG.CalendarFinder.Core.Models;

public record OutputData
{
	public required InputData Input { get; init; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public MyUser? User { get; init; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public MyCalendar[]? Calendars { get; init; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Error { get; init; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? InnerError { get; init; }
	[JsonIgnore]
	public User? MsUser { get; init; }
    [JsonIgnore] 
	public List<Calendar>? MsCalendars { get; init; }

	public OutputData WithUser(User user)
		=> this with
		{
			MsUser = user,
			User = new()
			{
				DisplayName = user.DisplayName,
				UserId = user.Id,
				Mail = user.Mail,
				PrincipalName = user.UserPrincipalName,
			},
		};

	public OutputData WithCalendars(List<Calendar> calendars)
		=> this with
		{
			User = (User ?? new()) with
			{
				CalendarId = GetDefaultCalendarId(calendars)
			},
			MsCalendars = calendars,
			Calendars = calendars
				.Select(e => new MyCalendar()
				{
					Id = e.Id,
					Name = e.Name,
					Owner = e.Owner?.Address,
					IsDefault = e.IsDefaultCalendar
				})
				.ToArray()
		};

	public OutputData WithError(Exception ex)
		=> this with
		{
			Error = GetErrorMessage(ex),
			InnerError = GetErrorMessage(ex.InnerException)
		};

	private static string? GetErrorMessage(Exception? ex)
		=> ex switch
		{
			ODataError oex => $"{oex.GetType().Name}: {oex.Error?.Message}",
			Exception e => $"{e.GetType().Name}: {e.Message}",
			_ => null
		};

	private static string? GetDefaultCalendarId(List<Calendar> calendars)
		=> calendars.FirstOrDefault(e => e.IsDefaultCalendar == true)?.Id
			?? calendars.FirstOrDefault()?.Id;
}

public record MyUser
{
	public string? DisplayName { get; init; }
	public string? PrincipalName { get; init; }
	public string? Mail { get; init; }
	public string? UserId { get; init; }
	public string? CalendarId { get; init; }
}

public record MyCalendar
{
	public string? Id { get; init; }
	public string? Name { get; init; }
	public string? Owner { get; init; }
	public bool? IsDefault { get; init; }
}