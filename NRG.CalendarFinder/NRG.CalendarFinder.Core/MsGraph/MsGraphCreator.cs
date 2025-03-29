using Azure.Identity;
using Microsoft.Graph;
using NRG.CalendarFinder.Core.Certificates;
using NRG.CalendarFinder.Core.Models;

namespace NRG.CalendarFinder.Core.MsGraph;

public class MsGraphCreator(
    ICertificateGet? get = null
    ) 
    : IMsGraphCreator
{
    private readonly ICertificateGet _loader = get ?? new WindowsCertificateHandler();

    public GraphServiceClient Create(IMsGraphCredential credentials)
        => Create(
            credentials.TenantId,
            credentials.ClientId,
            credentials.Thumbprint,
            credentials.Scopes
        );

    public GraphServiceClient Create(
        string tenantId,
        string clientId,
        string thumbprint,
        IEnumerable<string>? scopes = null
        )
    {
        var certificate = _loader.GetCertificate(thumbprint);

        var credential = new ClientCertificateCredential(
            tenantId: tenantId,
            clientId: clientId,
            clientCertificate: certificate,
            options: new() { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud }
        );

        return new(credential, scopes);
    }
}
