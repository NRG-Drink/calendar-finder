using Microsoft.Graph;

namespace NRG.CalendarFinder.Core.MsGraph;
public interface IMsGraphCreator
{
    GraphServiceClient Create(IMsGraphCredential credentials);
    GraphServiceClient Create(
        string tenantId,
        string clientId,
        string thumbprint,
        IEnumerable<string>? scopes = null
    );
}