using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Core.Certificates;

public class WindowsCertificateHandler(
    StoreName defaultStoreName = StoreName.My,
    StoreLocation defaultStoreLocation = StoreLocation.CurrentUser
    )
    : ICertificateCreate,
    ICertificateGet,
    ICertificateAdd,
    ICertificateRemove
{
    public X509Certificate2 CreateCertificate(
        string pfxFilePath,
        string? password = null,
        X509KeyStorageFlags flags = X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet
        )
        => new(pfxFilePath, password, flags);

    public X509Certificate2 GetCertificate(
        string thumbprint,
        StoreName? storeName = null,
        StoreLocation? storeLocation = null
        )
        => GetCertificatesFromStore(thumbprint, storeName ?? defaultStoreName, storeLocation ?? defaultStoreLocation)
            .FirstOrDefault()
            ?? throw new ArgumentNullException(
                $"No certificate found for {thumbprint} in " +
                $"store: {storeName}, location: {storeLocation}."
            );

    public void AddCertificate(
        X509Certificate2 certificate,
        StoreName? storeName = null,
        StoreLocation? storeLocation = null
        )
        => AddCertificateToStore(certificate, storeName ?? defaultStoreName, storeLocation ?? defaultStoreLocation);

    public void RemoveCertificate(
        X509Certificate2 certificate,
        StoreName? storeName = null,
        StoreLocation? storeLocation = null
        )
        => RemoveCertificateFromStore(certificate, storeName ?? defaultStoreName, storeLocation ?? defaultStoreLocation);

    private static X509Certificate2Collection GetCertificatesFromStore(
        string thumbprint,
        StoreName storeName,
        StoreLocation storeLocation
        )
    {
        using var store = new X509Store(storeName, storeLocation, OpenFlags.ReadOnly);

        var certificates = store.Certificates
            .Find(
                findType: X509FindType.FindByThumbprint,
                findValue: thumbprint,
                validOnly: false
            );

        return certificates;
    }

    private static void AddCertificateToStore(
        X509Certificate2 certificate,
        StoreName storeName,
        StoreLocation storeLocation
        )
    {
        using var store = new X509Store(storeName, storeLocation, OpenFlags.ReadWrite);

        store.Add(certificate);
    }

    private static void RemoveCertificateFromStore(
        X509Certificate2 certificate,
        StoreName storeName,
        StoreLocation storeLocation
        )
    {
        using var store = new X509Store(storeName, storeLocation, OpenFlags.ReadWrite);

        store.Remove(certificate);
    }
}
