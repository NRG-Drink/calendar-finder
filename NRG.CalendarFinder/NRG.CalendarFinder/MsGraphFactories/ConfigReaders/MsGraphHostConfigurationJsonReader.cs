using Microsoft.Extensions.Configuration;
using NRG.CalendarFinder.MsGraphFactories.Models;

namespace NRG.CalendarFinder.MsGraphFactories.ConfigReaders;

public class MsGraphHostConfigurationJsonReader(IConfiguration configuration)
{
	public IEnumerable<MsGraphCredentials> Read()
		=> configuration
			.GetSection("MsGraphClients")
			.GetChildren()
			.Select(ParseCredential);

	private MsGraphCredentials ParseCredential(IConfigurationSection clientSection)
		=> new()
		{
			//ClientName = clientSection.Key,
			ClientName = "GraphClient",
			TenantId = GetValue("TenantId", clientSection),
			ClientId = GetValue("ClientId", clientSection),
			Thumbprint = GetValue("Thumbprint", clientSection),
			Scopes = GetValues("Scopes", clientSection),
		};

	private string GetValue(string key, IConfigurationSection section)
		=> section.GetRequiredSection(key).Value
			?? $"No value found for: {key}";

	private string[] GetValues(string key, IConfigurationSection section)
		=> section
			.GetRequiredSection(key)
			.GetChildren()
			.Select(e => e.Value)
			.OfType<string>() // filter string?
			.ToArray();
}
