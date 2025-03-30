using FluentAssertions;
using NRG.CalendarFinder.Core.Certificates;
using NRG.CalendarFinder.Core.Tests.Certificates.DI;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Core.Tests.Certificates;
[Category("Local")]
[Category("Certificates")]
[WindowsCertificateDI]
public class CertificateTests(
    ICertificateCreate create,
    ICertificateGet get,
    ICertificateAdd add,
    ICertificateRemove remove
    )
{
    private static readonly string _dataFolder = Path.Combine("Certificates", "Data");

    public static IEnumerable<TestData> CertificateData()
    {
        yield return new("test-cert1", "test-cert1", "04C645B43054F9C90A393179A6D98C4E046D0B86");
        yield return new("test-cert2", "test-cert2", "50CEA2ECED00DE5D4B89F2C0DAA0AEF98819780E");
    }

    public record TestData(string FileName, string Password, string Thumbprint)
    {
        public string FilePath { get; } = new(Path.Combine(_dataFolder, $"{FileName}.pfx"));
        public override string ToString() => FileName;
    }

    [Test]
    [MatrixDataSource]
    public void CreateEx(
        [Matrix("wrong", "", null)] string? password,
        [Matrix("test-cert1", "test-cert2")] string fileName
        )
    {
        var filePath = Path.Combine(_dataFolder, $"{fileName}.pfx");

        var act = () => create.CreateCertificate(filePath, password);

        act.Should().ThrowExactly<CryptographicException>();
    }

    [Test]
    [NotInParallel]
    [MatrixDataSource]
    public async Task Store(
        [Matrix(StoreName.My, null)] StoreName? storeName,
        [Matrix(StoreLocation.CurrentUser, null)] StoreLocation? storeLocation,
        [MatrixMethod<CertificateTests>(nameof(CertificateData))] TestData e
        )
    {
        var cert = create.CreateCertificate(e.FilePath, e.Password);
        await Assert.That(cert.Subject).IsEqualTo($"CN={e.FileName}");
        await Assert.That(cert.Thumbprint).IsEqualTo(e.Thumbprint);

        add.AddCertificate(cert, storeName, storeLocation);

        var getCert = get.GetCertificate(e.Thumbprint, storeName, storeLocation);
        await Assert.That(getCert.Subject).IsEqualTo($"CN={e.FileName}");
        await Assert.That(getCert.Thumbprint).IsEqualTo(e.Thumbprint);

        remove.RemoveCertificate(cert, storeName, storeLocation);

        Assert.Throws(() => get.GetCertificate(e.Thumbprint, storeName, storeLocation));
    }
}
