using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.CertificateLoaders;

public interface ICertificateLoader
{
	public X509Certificate2 GetCertificate(string thumbprint);
}
