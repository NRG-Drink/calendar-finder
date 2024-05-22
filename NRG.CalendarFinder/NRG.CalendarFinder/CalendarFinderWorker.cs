using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using NRG.CalendarFinder.Models;
using System.Text.Json;

namespace NRG.CalendarFinder;

public class CalendarFinderWorker(
    IHost host,
    CalendarFinderService finder,
    ProcessData processData
    )
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Console.Out.WriteLineAsync($"Start Process with options: {processData.Options}");
        var processed = new List<OutputData>();
        foreach (var input in processData.Inputs)
        {
            var output = new OutputData() { Input = input };
            try
            {
                var user = await finder.FindUserAsyncOrThrow(input.UserIdentifier);
                var calendars = await finder.FindCalendarsAsyncOrThrow(user);

                output = GetOutputObject(output, user, calendars);
            }
            catch (ODataError ex)
            {
                var message = $"Failed with {nameof(ODataError)}: {ex.Error?.Message}";
                output = output with { Error = message };
            }
            catch (Exception ex)
            {
                var message = $"Failed with error: {ex.Message}";
                output = output with { Error = message };
            }
            finally
            {
                processed.Add(output);
                await Console.Out.WriteLineAsync($"Terminate search for: {input.UserIdentifier}");
            }
        }

        var path = GetResultPath(processData.Options.FilePath);
        await WriteToFile(path, processed);

        if (processData.Options.OpenEditor == true)
        {
            OpenVsCode(path);
        }

        await host.StopAsync(stoppingToken);
    }

    private static void OpenVsCode(string path)
    {
        var p = new System.Diagnostics.Process()
        {
            StartInfo = new()
            {
                UseShellExecute = true,
                FileName = "code",
                Arguments = path
            }
        };
        p.Start();
    }

    private static async Task WriteToFile(string path, List<OutputData> output)
    {
        var text = JsonSerializer.Serialize(output, options: new() { WriteIndented = true });
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

    private static Task WriteObjectAsync(string text, object? obj)
    {
        var json = JsonSerializer.Serialize(obj, options: new() { WriteIndented = true });
        return Console.Out.WriteLineAsync($"{text}:\n{json}");
    }

    private static OutputData GetOutputObject(OutputData output, User user, List<Calendar> calendars)
        => output with
        {
            User = new()
            {
                DisplayName = user.DisplayName,
                UserId = user.Id,
                Mail = user.Mail,
                PrincipalName = user.UserPrincipalName,
                CalendarId = GetDefaultCalendarId(calendars)
            },
            Calendars = calendars
            .Select(e => new MyCalendar()
            {
                Id = e.Id,
                Name = e.Name,
                Owner = e.Owner?.Address,
                IsDefault = e.IsDefaultCalendar
            })
            .ToArray()
        };

    private static string? GetDefaultCalendarId(List<Calendar> calendars)
    {
        if (calendars.Count == 1)
        {
            return calendars.Single().Id;
        }

        return calendars.FirstOrDefault(e => e.IsDefaultCalendar == true)?.Id;
    }
}
