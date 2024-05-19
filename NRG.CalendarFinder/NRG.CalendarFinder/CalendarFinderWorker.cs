using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using NRG.CalendarFinder.Models;
using System.Text.Json;

namespace NRG.CalendarFinder;

public class CalendarFinderWorker(
    IHost host,
    CalendarFinderService finder,
    Options options
    )
    : BackgroundService
{


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await WriteObjectAsync("Query", options);

            var user = await finder.FindUserAsyncOrThrow(options.UserIdentifier);
            var calendars = await finder.FindCalendarsAsyncOrThrow(user);

            await WriteObjectAsync("Result", GetPrintObject(user, calendars));
        }
        catch (ODataError ex)
        {
            await Console.Out.WriteLineAsync($"Failed with {nameof(ODataError)}: {ex.Error?.Message}");
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Failed with error: {ex.Message}");
        }
        finally
        {
            await host.StopAsync(stoppingToken);
        }
    }

    private static object GetPrintObject(User user, List<Calendar> calendars)
        => new
        {
            User = new { user.Id, user.Mail },
            Calendars = calendars.Select(e => new { e.Id, e.Name, Owner = e.Owner?.Address })
        };

    private static Task WriteObjectAsync(string text, object? obj)
    {
        var json = JsonSerializer.Serialize(obj, options: new() { WriteIndented = true });
        return Console.Out.WriteLineAsync($"{text}:\n{json}");
    }
}
