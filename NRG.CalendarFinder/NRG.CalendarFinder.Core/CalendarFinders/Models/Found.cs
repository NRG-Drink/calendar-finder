using Microsoft.Graph.Models;
using System.Collections.Immutable;

namespace NRG.CalendarFinder.Core.CalendarFinders.Models;

public record Found(string UserIdentifier, User User, ImmutableArray<Calendar> Calendars);
