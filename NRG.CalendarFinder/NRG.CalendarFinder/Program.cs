using CommandLine;
using NRG.CalendarFinder.Models;
using System.Text.Json;

namespace NRG.CalendarFinder;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(async options =>
            {
                await WriteObject("Start App with parameters", options);
                try
                {
                    var parser = new AppSettingsParser();
                    var credentials = await parser.ParseAppSettingsOrThrow(options?.AppSettingsPath);

                    var calendarFinder = new CalendarFinder();
                    await calendarFinder.FindCalendars(options);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Failed with error: {ex.Message}");
                }
                finally
                {
                    await Console.Out.WriteLineAsync($"Terminate App.");
                }
            });
    }

    private static Task WriteObject(string text, object? obj)
    {
        var json = JsonSerializer.Serialize(obj, options: new() { WriteIndented = true });
        return Console.Out.WriteLineAsync($"{text}:\n{json}");
    }
}
