using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Core.Certificates;

public interface ICertificateRemove
{
    void RemoveCertificate(
        X509Certificate2 certificate,
        StoreName? storeName = null,
        StoreLocation? storeLocation = null
    );
}
