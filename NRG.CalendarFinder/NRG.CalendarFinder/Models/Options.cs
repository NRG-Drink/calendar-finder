using CommandLine;

namespace NRG.CalendarFinder.Models;

public record Options
{
	[Option('a', "app-settings", Required = true, HelpText = "Name or path to the file. (e.g. my-data.json)")]
	public required string FilePath { get; init; }
	[Option('o', "open-editor", Required = false, HelpText = "Open the editor after finish.")]
	public bool? OpenEditor { get; init; } = true;
}
