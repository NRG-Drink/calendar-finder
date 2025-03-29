using NRG.CalendarFinder.Core.MsGraph;

namespace NRG.CalendarFinder.Core.Models;

public record AppSettings
{
    public required MsGraphCredential MsGraphCredential { get; init; }
    public string[] UserIdentifiers { get; init; } = [];
}
