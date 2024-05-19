using Azure.Identity;
using Microsoft.Graph;
using NRG.CalendarFinder.CertificateLoaders;
using NRG.CalendarFinder.MsGraphFactories.Models;

namespace NRG.CalendarFinder.MsGraphFactories;

public class MsGraphClientFactory : IMsGraphClientFactory
{
    private readonly Dictionary<string, MsGraphCredentials> _credentialStore = new();
    private readonly Dictionary<string, GraphServiceClient> _clientStore = new();
    private readonly ICertificateLoader _certificateLoader;


    public MsGraphClientFactory(ICertificateLoader certificateLoader)
    {
        _certificateLoader = certificateLoader;
    }


    public void AddCredential(MsGraphCredentials credential)
    {
        ThrowIfClientNameIsDuplicate(credential);
        _credentialStore.Add(credential.ClientName, credential);
    }

    public GraphServiceClient GetClient(string name)
    {
        if (_clientStore.TryGetValue(name, out var client))
        {
            return client;
        }
        else
        {
            if (_credentialStore.TryGetValue(name, out var credential))
            {
                client = CreateClient(credential);
                _clientStore.Add(credential.ClientName, client);
                _credentialStore.Remove(name);
                return client;
            }

            throw new NullReferenceException(
                $"No GraphServiceClient could be found for key: '{name}'"
            );
        }
    }

    private void ThrowIfClientNameIsDuplicate(MsGraphCredentials credential)
    {
        if (_credentialStore.TryGetValue(credential.ClientName, out var _))
        {
            throw new ArgumentException(
                $"There is already an credential with this name. ({credential.ClientName})"
            );
        }
    }

    private GraphServiceClient CreateClient(MsGraphCredentials credentials)
    {
        var msGraphCredentials = GetCertificateCredential(credentials);

        return new GraphServiceClient(msGraphCredentials, credentials.Scopes);
    }

    private ClientCertificateCredential GetCertificateCredential(
        MsGraphCredentials credentials
        )
    {
        var certificate = _certificateLoader.GetCertificate(credentials.Thumbprint);
        var options = GetOptions();

        return new ClientCertificateCredential(
            tenantId: credentials.TenantId,
            clientId: credentials.ClientId,
            clientCertificate: certificate,
            options: options
            );
    }

    private TokenCredentialOptions GetOptions()
        => new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };
}
