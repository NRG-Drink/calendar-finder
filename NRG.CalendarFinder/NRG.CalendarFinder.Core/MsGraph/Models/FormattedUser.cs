namespace NRG.CalendarFinder.Core.MsGraph.Models;

public record FormattedUser
{
    public string? UserId { get; init; }
    public string? DisplayName { get; init; }
    public string? PrincipalName { get; init; }
    public string? Mail { get; init; }
    public string? DefaultCalendarId { get; init; }
    public string? DefaultCalendarName { get; init; }
}
