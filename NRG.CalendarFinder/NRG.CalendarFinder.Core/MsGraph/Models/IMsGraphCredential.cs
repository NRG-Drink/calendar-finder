namespace NRG.CalendarFinder.Core.MsGraph.Models;

public interface IMsGraphCredential
{
    public string TenantId { get; }
    public string ClientId { get; }
    public string Thumbprint { get; }
    public IEnumerable<string>? Scopes { get; }
}

