namespace NRG.CalendarFinder.Tests.AppSettings;
[Trait("Category", "Unit")]
[Trait("AppSettings", "Unit")]
public class AppSettingsParserTests
{
    [Theory]
    [InlineData("single1.json", "he", "ll", "lo")]
    [InlineData("single2.txt", "up", "down", "lo")]
    [InlineData("single3", "hello", "world", "!")]
    public async Task ParseSingle(
        string file,
        string? tenantId,
        string? clientId,
        string? thumbprint
        )
    {
        var path = Path.Combine("AppSettings", "Data", file);

        var parser = new AppSettingsParser();
        var credential = await parser.ParseAppSettingsOrThrow(path);

        Assert.Equal(tenantId, credential.TenantId);
        Assert.Equal(clientId, credential.ClientId);
        Assert.Equal(thumbprint, credential.Thumbprint);
    }

    [Theory]
    [InlineData("hello.world")]
    [InlineData("singleEx1.json")]
    [InlineData("singleEx2.json")]
    [InlineData("singleEx3.json")]
    [InlineData("singleEx4.json")]
    [InlineData("singleEx5.json")]
    [InlineData("singleEx6.json")]
    [InlineData("singleEx7.json")]
    public async Task ParseEx(string file)
    {
        var path = Path.Combine("AppSettings", "Data", file);

        var parser = new AppSettingsParser();
        await Assert.ThrowsAnyAsync<ArgumentException>(() => parser.ParseAppSettingsOrThrow(path));
    }
}
