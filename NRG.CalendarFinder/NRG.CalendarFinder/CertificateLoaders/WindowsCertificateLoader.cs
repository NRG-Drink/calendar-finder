using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.CertificateLoaders;

public class WindowsCertificateLoader : ICertificateLoader
{
    private readonly StoreName _storeName;
    private readonly StoreLocation _storeLocation;


    public WindowsCertificateLoader(
        StoreName storeName = StoreName.My,
        StoreLocation storeLocation = StoreLocation.CurrentUser
        )
    {
        _storeName = storeName;
        _storeLocation = storeLocation;
    }


    public X509Certificate2 GetCertificate(string thumbprint)
    {
        var certificates = GetCertificatesFromStore(thumbprint);
        var cert = certificates.FirstOrDefault();

        return cert
            ?? throw new NullReferenceException(
                $"No certificate found for {thumbprint} in " +
                $"store: {_storeName}, location: {_storeLocation}."
            );
    }

    private X509Certificate2Collection GetCertificatesFromStore(string thumbprint)
    {
        var store = new X509Store(_storeName, _storeLocation);
        store.Open(OpenFlags.ReadOnly);
        var certificates = store.Certificates
            .Find(X509FindType.FindByThumbprint, thumbprint, false);
        store.Close();
        return certificates;
    }
}
