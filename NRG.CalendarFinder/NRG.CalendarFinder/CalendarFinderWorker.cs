using Microsoft.Extensions.Hosting;
using NRG.CalendarFinder.Core.CalendarFinders;
using NRG.CalendarFinder.Core.Models;
using NRG.CalendarFinder.Core.MsGraph;
using NRG.CalendarFinder.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NRG.CalendarFinder;

public class CalendarFinderWorker(
    IHost host,
    Options options,
    AppSettings settings,
    ICalendarFinder calfi,
    MsGraphFormatter formatter
    )
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Console.Out.WriteLineAsync($"Start Process with {options}");

        var tasks = settings.UserIdentifiers.Select(calfi.FindCalendarAsync);
        var processed = await Task.WhenAll(tasks);

        var path = GetResultPath(options.FilePath);
        var formatted = processed.Select(formatter.Format);
        await WriteToFileAsync(path, formatted);

        if (options.OpenEditor == true)
        {
            OpenVsCode(path);
        }

        await host.StopAsync(stoppingToken);
    }

    private static async Task WriteToFileAsync(string path, IEnumerable<FormattedResponse> result)
    {
        var text = JsonSerializer.Serialize(result, options: new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        await File.WriteAllTextAsync(path, text);
        await Console.Out.WriteLineAsync($"Wrote output data to file {path}.");
    }

    private static string GetResultPath(string inputPath)
    {
        var name = Path.GetFileNameWithoutExtension(inputPath);
        var dir = Path.GetPathRoot(inputPath)
            ?? throw new ArgumentNullException(
                $"No root directory for file {inputPath} found. " +
                $"Output file could not be written."
            );
        var path = Path.Combine(dir, $"{name}.result.json");
        return path;
    }

    private static void OpenVsCode(string path)
    {
        var process = new System.Diagnostics.Process()
        {
            StartInfo = new()
            {
                UseShellExecute = true,
                FileName = "code",
                Arguments = path
            }
        };
        process.Start();
    }
}
