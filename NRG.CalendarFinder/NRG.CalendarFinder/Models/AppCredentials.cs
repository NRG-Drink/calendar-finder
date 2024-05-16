namespace NRG.CalendarFinder.Models;

public record AppCredentials
{
    public string TenantId { get; init; } = "missing tenant-id";
    public string ClientId { get; init; } = "missing client-id";
    public string Thumbprint { get; init; } = "missing thumbprint";
}
