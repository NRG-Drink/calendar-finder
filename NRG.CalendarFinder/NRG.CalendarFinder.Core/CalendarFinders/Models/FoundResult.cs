using Microsoft.Graph.Models;
using System.Collections.Immutable;

namespace NRG.CalendarFinder.Core.CalendarFinders.Models;

public record FoundResult(string UserIdentifier, User User, ImmutableArray<Calendar> Calendars);
public record FoundException(string UserIdentifier, Exception Error);
