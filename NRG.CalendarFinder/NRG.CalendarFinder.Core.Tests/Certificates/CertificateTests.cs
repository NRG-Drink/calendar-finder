using FluentAssertions;
using NRG.CalendarFinder.Core.Certificates;
using NRG.CalendarFinder.Core.Tests.Certificates.DI;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NRG.CalendarFinder.Core.Tests.Certificates;
[Category("Local")]
[WindowsCertificateLoaderDI]
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
    [MatrixDataSource]
    public void Create(
        [MatrixMethod<CertificateTests>(nameof(CertificateData))] TestData e
        )
    {
        var cert = create.CreateCertificate(e.FilePath, e.Password);

        cert.FriendlyName.Should().Be(e.FileName);
        cert.Thumbprint.Should().Be(e.Thumbprint);
    }

    [Test, DependsOn(nameof(Create))]
    [MatrixDataSource]
    public void Add(
        [Matrix(StoreName.My, null)] StoreName? storeName,
        [Matrix(StoreLocation.CurrentUser, null)] StoreLocation? storeLocation,
        [MatrixMethod<CertificateTests>(nameof(CertificateData))] TestData e
        )
    {
        var cert = create.CreateCertificate(e.FilePath, e.Password);

        add.AddCertificate(cert, storeName, storeLocation);
    }

    [Test, DependsOn(nameof(Add))]
    [MatrixDataSource]
    public void Get(
        [Matrix(StoreName.My, null)] StoreName? storeName,
        [Matrix(StoreLocation.CurrentUser, null)] StoreLocation? storeLocation,
        [MatrixMethod<CertificateTests>(nameof(CertificateData))] TestData e
        )
    {
        var cert = get.GetCertificate(e.Thumbprint, storeName, storeLocation);

        cert.FriendlyName.Should().Be(e.FileName);
        cert.Thumbprint.Should().Be(e.Thumbprint);
    }

    [Test, DependsOn(nameof(Get), ProceedOnFailure = true)]
    [MatrixDataSource]
    public void Remove(
        [Matrix(StoreName.My, null)] StoreName? storeName,
        [Matrix(StoreLocation.CurrentUser, null)] StoreLocation? storeLocation,
        [MatrixMethod<CertificateTests>(nameof(CertificateData))] TestData e
        )
    {
        var cert = get.GetCertificate(e.Thumbprint, storeName, storeLocation);

        remove.RemoveCertificate(cert, storeName, storeLocation);
        var act = () => get.GetCertificate(e.Thumbprint, storeName, storeLocation);

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
