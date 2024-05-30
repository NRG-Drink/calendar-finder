namespace NRG.CalendarFinder.MsGraphFactories.Models;

public record MsGraphCredentials
{
	public required string ClientName { get; set; }
	public required string TenantId { get; set; }
	public required string ClientId { get; set; }
	public required string Thumbprint { get; set; }
	public required string[] Scopes { get; set; }
}
