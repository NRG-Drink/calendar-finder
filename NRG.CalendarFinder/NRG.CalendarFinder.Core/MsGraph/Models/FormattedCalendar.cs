namespace NRG.CalendarFinder.Core.MsGraph.Models;

public record FormattedCalendar
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Owner { get; init; }
    public bool? IsDefault { get; init; }
}
