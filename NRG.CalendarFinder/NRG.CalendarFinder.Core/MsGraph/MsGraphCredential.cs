namespace NRG.CalendarFinder.Core.MsGraph;

public record MsGraphCredential(
    string ClientId,
    string TenantId,
    string Thumbprint,
    IEnumerable<string>? Scopes
    )
    : IMsGraphCredential;
//{
//    public required string ClientId { get; init; }
//    public required string TenantId { get; init; }
//    public required string Thumbprint { get; init; }
//    public IEnumerable<string>? Scopes { get; init; }
//}

