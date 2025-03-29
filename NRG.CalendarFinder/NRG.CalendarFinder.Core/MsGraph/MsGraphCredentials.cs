namespace NRG.CalendarFinder.Core.Models;

public record MsGraphCredentials : IMsGraphCredential
{
    public required string ClientId { get; init; }
    public required string TenantId { get; init; }
    public required string Thumbprint { get; init; }
    public IEnumerable<string>? Scopes { get; init; }
}

