using CommandLine;

namespace NRG.CalendarFinder.Models;

public record Options
{

    [Option('u', "user", Required = true, HelpText = "Can be a GUID or the mail address")]
    public string? UserIdentifier { get; set; }
    [Option('a', "appsettings", Required = false, HelpText = "Relative path and filename of appsettings.json")]
    public string AppSettingsPath { get; set; } = "calendar-finder-appsettings.json";
}
