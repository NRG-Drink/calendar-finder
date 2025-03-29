using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Core.Certificates;

public interface ICertificateGet
{
    X509Certificate2 GetCertificate(
        string thumbprint,
        StoreName? storeName = null,
        StoreLocation? storeLocation = null
    );
}
