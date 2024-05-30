using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRG.CalendarFinder.CertificateLoaders;
using NRG.CalendarFinder.MsGraphFactories;
using NRG.CalendarFinder.MsGraphFactories.ConfigReaders;
using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Extensions;

public static class IHostBuilderExtensionsMsGraphClientFactory
{
	public static IHostBuilder AddGraphClientsFromJson(this IHostBuilder builder)
	{
		builder.ConfigureServices((context, services) =>
		{
			var reader = new MsGraphHostConfigurationJsonReader(context.Configuration);
			var credentials = reader.Read();

			var factory = new MsGraphClientFactory(GetCertificateLoader());
			credentials.ToList().ForEach(factory.AddCredential);

			services.AddSingleton<IMsGraphClientFactory>(factory);
		});

		return builder;
	}

	private static WindowsCertificateLoader GetCertificateLoader()
		=> new(StoreName.My, StoreLocation.CurrentUser);
}