using CommandLine;

namespace NRG.CalendarFinder.Models;

public record Options
{
    [Option('f', "file", Required = true, HelpText = "Name or path to the file. (e.g. mydata.json)")]
    public required string FilePath { get; init; }
    [Option('e', "open-editor", Required = false, HelpText = "Open the editor after finish.")]
    public bool? OpenEditor { get; init; } = true;
}
