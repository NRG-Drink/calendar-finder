using Microsoft.Extensions.Configuration;
using NRG.CalendarFinder.Core.MsGraphFactories.Models;

namespace NRG.CalendarFinder.Core.MsGraphFactories.ConfigReaders;

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
			ClientName = "GraphClient",
			TenantId = GetValue("TenantId", clientSection),
			ClientId = GetValue("ClientId", clientSection),
			Thumbprint = GetValue("Thumbprint", clientSection),
			Scopes = GetValues("Scopes", clientSection),
		};

	private static string GetValue(string key, IConfigurationSection section)
		=> section.GetRequiredSection(key).Value
			?? $"No value found for: {key}";

	private static string[] GetValues(string key, IConfigurationSection section)
		=> section.GetRequiredSection(key).GetChildren().Select(e => e.Value).OfType<string>().ToArray();
}
