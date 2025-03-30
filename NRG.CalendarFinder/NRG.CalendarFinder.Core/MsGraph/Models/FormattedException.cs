namespace NRG.CalendarFinder.Core.MsGraph.Models;

public record FormattedException
{
    public required string Type { get; init; }
    public required string Message { get; init; }
    public FormattedException? InnerException { get; init; }
}
