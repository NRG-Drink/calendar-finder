using NRG.CalendarFinder.Models;
using System.Text.Json;

namespace NRG.CalendarFinder;

public class AppSettingsParser
{
    public async Task<AppCredentials> ParseAppSettingsOrThrow(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException(
                "No config file found to read Azure AD App credentials",
                path
            );
        }

        return await ParseFile(path);
    }

    private static async Task<AppCredentials> ParseFile(string path)
    {
        AppCredentials? credentials;
        try
        {
            var text = await File.ReadAllTextAsync(path);
            credentials = JsonSerializer.Deserialize<AppCredentials>(text);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                $"Something went wrong by parsing the appsettings-file",
                path,
                ex
            );
        }

        if (IsNotValid(credentials))
        {
            throw new ArgumentException($"No valid Azure AD App credentials could be parsed.", path);
        }

        return credentials!;
    }

    private static bool IsNotValid(AppCredentials? c)
        => string.IsNullOrWhiteSpace(c?.ClientId)
        || string.IsNullOrWhiteSpace(c?.TenantId)
        || string.IsNullOrWhiteSpace(c?.Thumbprint);
}