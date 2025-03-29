using Microsoft.Graph.Models;
using OneOf;

namespace NRG.CalendarFinder.Core.CalendarFinders;

public interface ICalendarService
{
    Task<OneOf<List<Calendar>, Exception>> FindCalendarAsync(User user);
}
