using CommandLine;

namespace NRG.CalendarFinder.Models;

public record Options
{
    [Option('u', "user", Required = true, HelpText = "Can be a Guid or the user-principal-name (mail-address)")]
    public required string UserIdentifier { get; set; }
    [Option('a', "appsettings", Required = false, HelpText = "Path and filename of appsettings.json")]
    public string AppSettingsPath { get; set; } = "calendar-finder-appsettings.json";
}
