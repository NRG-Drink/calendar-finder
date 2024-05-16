using CommandLine;
using NRG.CalendarFinder.Models;

namespace NRG.CalendarFinder;

internal class Program
{
    static async Task Main(string[] args)
    {
        var parsedArgs = Parser.Default.ParseArguments<Options>(args);

        var calendarFinder = new CalendarFinder();
        await calendarFinder.FindCalendars(parsedArgs.Value);
    }
}
