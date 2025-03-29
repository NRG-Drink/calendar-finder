using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Core.Certificates;

public interface ICertificateCreate
{
    X509Certificate2 CreateCertificate(
        string pfxFilePath,
        string? password = null,
        X509KeyStorageFlags flags = 0
    );
}
