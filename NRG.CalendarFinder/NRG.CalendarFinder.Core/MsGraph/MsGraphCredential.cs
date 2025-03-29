namespace NRG.CalendarFinder.Core.MsGraph;

public record MsGraphCredential(
    string ClientId,
    string TenantId,
    string Thumbprint,
    IEnumerable<string>? Scopes = null
    )
    : IMsGraphCredential;
