using NRG.CalendarFinder.Core.MsGraph.Models;

namespace NRG.CalendarFinder.Core.MsGraph;

public record FormattedResponse(
    string UserIdentifier,
    FormattedUser? User,
    FormattedCalendar? DefaultCalendar,
    FormattedCalendar[]? Calendars,
    FormattedException? Error
    );
